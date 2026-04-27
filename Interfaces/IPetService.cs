using VeterinarySystem.Models;
using VeterinarySystem.Responses;

namespace VeterinarySystem.Interfaces;

public interface IPetService
{
    Task<ServiceResponse<List<Pet>>> GetAllAsync();
    Task<ServiceResponse<List<Pet>>> GetByOwnerAsync(int ownerId);
    Task<ServiceResponse<Pet>> GetByIdAsync(int id);
    Task<ServiceResponse<Pet>> CreateAsync(Pet pet);
    Task<ServiceResponse<Pet>> UpdateAsync(int id, Pet pet);
    Task<ServiceResponse<bool>> DeleteAsync(int id);
}