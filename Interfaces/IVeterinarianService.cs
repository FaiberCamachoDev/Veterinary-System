using VeterinarySystem.Models;
using VeterinarySystem.Responses;

namespace VeterinarySystem.Interfaces;

public interface IVeterinarianService
{
    Task<ServiceResponse<List<Veterinarian>>> GetAllAsync();
    Task<ServiceResponse<Veterinarian>> GetByIdAsync(int id);
    Task<ServiceResponse<Veterinarian>> CreateAsync(Veterinarian veterinarian);
    Task<ServiceResponse<Veterinarian>> UpdateAsync(Veterinarian veterinarian);
    Task<ServiceResponse> DeleteAsync(int id);
}