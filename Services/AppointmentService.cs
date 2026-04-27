using Microsoft.EntityFrameworkCore;
using VeterinarySystem.Data;
using VeterinarySystem.Interfaces;
using VeterinarySystem.Models;
using VeterinarySystem.Responses;

namespace VeterinarySystem.Services;

public class AppointmentService : IAppointmentService
{
    private readonly VeterinaryContext _context;
    private readonly IEmailService _emailService; // Inyectamos el servicio de correos que vimos antes

    public AppointmentService(VeterinaryContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<ServiceResponse<List<Appointment>>> GetAllAsync()
    {
        // Usamos Include para traer los datos relacionados y poder mostrar sus nombres en la vista
        var appointments = await _context.Appointments
            .Include(a => a.Pet)
            .Include(a => a.Owner)
            .Include(a => a.Veterinarian)
            .OrderByDescending(a => a.Date)
            .ThenBy(a => a.StartTime)
            .ToListAsync();

        return ServiceResponse<List<Appointment>>.Ok(appointments);
    }

    public async Task<ServiceResponse<Appointment>> GetByIdAsync(int id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Pet)
            .Include(a => a.Owner)
            .Include(a => a.Veterinarian)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null) return ServiceResponse<Appointment>.Fail("Cita no encontrada.");

        return ServiceResponse<Appointment>.Ok(appointment);
    }

    public async Task<ServiceResponse<Appointment>> CreateAsync(Appointment appointment)
    {
        // 1. Validar Dueño y Bloqueos
        var owner = await _context.Owners.FindAsync(appointment.OwnerId);
        if (owner == null) return ServiceResponse<Appointment>.Fail("Dueño no encontrado.");
        
        if (owner.IsBlocked && owner.BlockedUntil > DateTime.UtcNow)
        {
            return ServiceResponse<Appointment>.Fail($"El cliente está bloqueado hasta {owner.BlockedUntil:dd/MM/yyyy}. No puede agendar citas.");
        }

        // 2. Validar Veterinario (Horarios de trabajo)
        var vet = await _context.Veterinarians.FindAsync(appointment.VeterinarianId);
        if (vet == null) return ServiceResponse<Appointment>.Fail("Veterinario no encontrado.");

        if (appointment.StartTime < vet.AvailableFrom || appointment.EndTime > vet.AvailableTo)
        {
            return ServiceResponse<Appointment>.Fail($"El horario del Dr(a). {vet.Name} es de {vet.AvailableFrom:HH:mm} a {vet.AvailableTo:HH:mm}.");
        }

        // 3. Validar Solapamiento de Citas
        // Verificamos si existe alguna cita ese mismo día que se cruce con las horas seleccionadas
        var hasOverlap = await _context.Appointments
            .AnyAsync(a => a.VeterinarianId == vet.Id 
                        && a.Date == appointment.Date
                        && a.Status != AppointmentStatus.Cancelled // Si está cancelada, no estorba
                        && a.StartTime < appointment.EndTime 
                        && a.EndTime > appointment.StartTime);

        if (hasOverlap)
        {
            return ServiceResponse<Appointment>.Fail("El veterinario ya tiene una cita ocupando ese bloque de tiempo.");
        }

        // 4. Guardar Cita
        appointment.Status = AppointmentStatus.Scheduled;
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        // 5. Enviar Correo Electrónico
        var pet = await _context.Pets.FindAsync(appointment.PetId);
        if (pet != null)
        {
            await _emailService.SendAppointmentCreatedAsync(
                owner.Email, owner.Name, pet.Name, appointment.Date, appointment.StartTime);
        }

        return ServiceResponse<Appointment>.Ok(appointment, "Cita programada con éxito y correo de confirmación enviado.");
    }

    public async Task<ServiceResponse> CancelAsync(int id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Owner)
            .Include(a => a.Pet)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null) return ServiceResponse.Fail("Cita no encontrada.");
        
        if (appointment.Status == AppointmentStatus.Cancelled)
            return ServiceResponse.Fail("La cita ya había sido cancelada.");

        // Solo se pueden cancelar citas futuras
        if (appointment.Date < DateTime.UtcNow.Date)
            return ServiceResponse.Fail("No se pueden cancelar citas de fechas pasadas.");

        appointment.Status = AppointmentStatus.Cancelled;
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();

        // Enviar correo de cancelación
        await _emailService.SendAppointmentCancelledAsync(
            appointment.Owner.Email, appointment.Owner.Name, appointment.Pet.Name, appointment.Date);

        return ServiceResponse.Ok("Cita cancelada con éxito y cliente notificado.");
    }
}