namespace VeterinarySystem.Models;

public class Medication
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;   // disponibilidad simulada

    // Navegación
    public ICollection<TreatmentMedication> TreatmentMedications { get; set; } 
        = new List<TreatmentMedication>();
}