using VeterinarySystem.Models;
using VeterinarySystem.Responses;

namespace VeterinarySystem.Interfaces;

public interface IOwnerService
{
    Task<ServiceResponse<List<Owner>>> GetAllAsync();
    Task<ServiceResponse<Owner>> GetByIdAsync(int id);
    Task<ServiceResponse<Owner>> CreateAsync(Owner owner);
    Task<ServiceResponse<Owner>> UpdateAsync(int id, Owner owner);
    Task<ServiceResponse<bool>> DeleteAsync(int id);
    Task<ServiceResponse<bool>> ExistsByDocumentAsync(string document, int? excludeId = null);
    Task<ServiceResponse<bool>> ExistsByEmailAsync(string email, int? excludeId = null);
}