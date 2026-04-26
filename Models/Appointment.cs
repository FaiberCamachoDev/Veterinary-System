namespace VeterinarySystem.Models;

public enum AppointmentStatus
{
    Scheduled,    // Programada
    Cancelled,    // Cancelada
    Attended,     // Atendida
    NoShow        // No asistió
}

public class Appointment
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string? Notes { get; set; }

    // FKs
    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;

    public int OwnerId { get; set; }
    public Owner Owner { get; set; } = null!;

    public int VeterinarianId { get; set; }
    public Veterinarian Veterinarian { get; set; } = null!;

    // Navegación
    public Treatment? Treatment { get; set; }
}