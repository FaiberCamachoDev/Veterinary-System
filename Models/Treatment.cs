namespace VeterinarySystem.Models;

public class Treatment
{
    public int Id { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string Observations { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    // FK (1 a 1 con Appointment)
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;

    // Navegación
    public ICollection<TreatmentMedication> TreatmentMedications { get; set; } 
        = new List<TreatmentMedication>();
}