using Veterinary_System.Models;

namespace VeterinarySystem.Models;

public class Owner
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;   // único
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;      // único
    public bool IsBlocked { get; set; } = false;
    public DateTime? BlockedUntil { get; set; }

    // Navegación
    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
}