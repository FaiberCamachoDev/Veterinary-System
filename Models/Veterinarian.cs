namespace VeterinarySystem.Models;

public class Veterinarian
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public TimeOnly AvailableFrom { get; set; }            // Ej: 08:00
    public TimeOnly AvailableTo { get; set; }              // Ej: 18:00

    // Navegación
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}