namespace VeterinarySystem.Models;

public class TreatmentMedication
{
    public int Id { get; set; }
    public string Dose { get; set; } = string.Empty;        // Ej: "500mg"
    public string Frequency { get; set; } = string.Empty;   // Ej: "Cada 8 horas"

    // FKs
    public int TreatmentId { get; set; }
    public Treatment Treatment { get; set; } = null!;

    public int MedicationId { get; set; }
    public Medication Medication { get; set; } = null!;
}