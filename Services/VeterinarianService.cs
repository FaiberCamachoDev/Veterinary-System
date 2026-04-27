using Microsoft.EntityFrameworkCore;
using VeterinarySystem.Data;
using VeterinarySystem.Interfaces;
using VeterinarySystem.Models;
using VeterinarySystem.Responses;

namespace VeterinarySystem.Services;

public class VeterinarianService : IVeterinarianService
{
    private readonly VeterinaryContext _context;

    public VeterinarianService(VeterinaryContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<List<Veterinarian>>> GetAllAsync()
    {
        var vets = await _context.Veterinarians.ToListAsync();
        return ServiceResponse<List<Veterinarian>>.Ok(vets);
    }

    public async Task<ServiceResponse<Veterinarian>> GetByIdAsync(int id)
    {
        var vet = await _context.Veterinarians.FindAsync(id);
        if (vet == null)
            return ServiceResponse<Veterinarian>.Fail("Veterinario no encontrado.");

        return ServiceResponse<Veterinarian>.Ok(vet);
    }

    public async Task<ServiceResponse<Veterinarian>> CreateAsync(Veterinarian veterinarian)
    {
        try
        {
            _context.Veterinarians.Add(veterinarian);
            await _context.SaveChangesAsync();
            return ServiceResponse<Veterinarian>.Ok(veterinarian, "Veterinario creado con éxito.");
        }
        catch (DbUpdateException)
        {
            // Capturamos el error de unicidad que configuramos en el DbContext (Nombre + Especialidad)
            return ServiceResponse<Veterinarian>.Fail("Ya existe un veterinario con ese nombre y especialidad.");
        }
    }

    public async Task<ServiceResponse<Veterinarian>> UpdateAsync(Veterinarian veterinarian)
    {
        _context.Veterinarians.Update(veterinarian);
        await _context.SaveChangesAsync();
        return ServiceResponse<Veterinarian>.Ok(veterinarian, "Veterinario actualizado con éxito.");
    }

    public async Task<ServiceResponse> DeleteAsync(int id)
    {
        // Incluimos las citas para poder verificar la regla de negocio
        var vet = await _context.Veterinarians
            .Include(v => v.Appointments)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vet == null) return ServiceResponse.Fail("Veterinario no encontrado.");

        // Regla de Negocio: Validar si tiene citas programadas (Scheduled)
        bool hasScheduledAppointments = vet.Appointments.Any(a => a.Status == AppointmentStatus.Scheduled);
        
        if (hasScheduledAppointments)
        {
            return ServiceResponse.Fail("No se puede eliminar el veterinario porque tiene citas programadas a futuro.");
        }

        _context.Veterinarians.Remove(vet);
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Veterinario eliminado correctamente.");
    }
}