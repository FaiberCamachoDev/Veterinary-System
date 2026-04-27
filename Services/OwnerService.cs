using Microsoft.EntityFrameworkCore;
using VeterinarySystem.Data;
using VeterinarySystem.Interfaces;
using VeterinarySystem.Models;
using VeterinarySystem.Responses;

namespace VeterinarySystem.Services;

public class OwnerService : IOwnerService
{
    private readonly VeterinaryContext _context;

    public OwnerService(VeterinaryContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<List<Owner>>> GetAllAsync()
    {
        try
        {
            var owners = await _context.Owners
                .Include(o => o.Pets)
                .OrderBy(o => o.Name)
                .ToListAsync();

            return ServiceResponse<List<Owner>>.Ok(owners);
        }
        catch (Exception ex)
        {
            return ServiceResponse<List<Owner>>.Fail($"Error al obtener propietarios: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<Owner>> GetByIdAsync(int id)
    {
        try
        {
            var owner = await _context.Owners
                .Include(o => o.Pets)
                .Include(o => o.Appointments)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (owner is null)
                return ServiceResponse<Owner>.Fail("Propietario no encontrado.");

            return ServiceResponse<Owner>.Ok(owner);
        }
        catch (Exception ex)
        {
            return ServiceResponse<Owner>.Fail($"Error al obtener propietario: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<Owner>> CreateAsync(Owner owner)
    {
        try
        {
            var errors = new List<string>();

            if (await _context.Owners.AnyAsync(o => o.Document == owner.Document))
                errors.Add($"Ya existe un propietario con el documento '{owner.Document}'.");

            if (await _context.Owners.AnyAsync(o => o.Email == owner.Email))
                errors.Add($"Ya existe un propietario con el email '{owner.Email}'.");

            if (errors.Count > 0)
                return ServiceResponse<Owner>.Fail(errors);

            _context.Owners.Add(owner);
            await _context.SaveChangesAsync();

            return ServiceResponse<Owner>.Ok(owner, "Propietario registrado exitosamente.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<Owner>.Fail($"Error al crear propietario: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<Owner>> UpdateAsync(int id, Owner owner)
    {
        try
        {
            var existing = await _context.Owners.FindAsync(id);

            if (existing is null)
                return ServiceResponse<Owner>.Fail("Propietario no encontrado.");

            var errors = new List<string>();

            if (await _context.Owners.AnyAsync(o => o.Document == owner.Document && o.Id != id))
                errors.Add($"Ya existe otro propietario con el documento '{owner.Document}'.");

            if (await _context.Owners.AnyAsync(o => o.Email == owner.Email && o.Id != id))
                errors.Add($"Ya existe otro propietario con el email '{owner.Email}'.");

            if (errors.Count > 0)
                return ServiceResponse<Owner>.Fail(errors);

            existing.Name = owner.Name;
            existing.Document = owner.Document;
            existing.Phone = owner.Phone;
            existing.Email = owner.Email;

            await _context.SaveChangesAsync();

            return ServiceResponse<Owner>.Ok(existing, "Propietario actualizado exitosamente.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<Owner>.Fail($"Error al actualizar propietario: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(int id)
    {
        try
        {
            var owner = await _context.Owners
                .Include(o => o.Pets)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (owner is null)
                return ServiceResponse<bool>.Fail("Propietario no encontrado.");

            if (owner.Pets.Count > 0)
                return ServiceResponse<bool>.Fail(
                    "No se puede eliminar el propietario porque tiene mascotas registradas.");

            _context.Owners.Remove(owner);
            await _context.SaveChangesAsync();

            return ServiceResponse<bool>.Ok(true, "Propietario eliminado exitosamente.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Fail($"Error al eliminar propietario: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> ExistsByDocumentAsync(string document, int? excludeId = null)
    {
        var exists = await _context.Owners
            .AnyAsync(o => o.Document == document && (excludeId == null || o.Id != excludeId));
        return ServiceResponse<bool>.Ok(exists);
    }

    public async Task<ServiceResponse<bool>> ExistsByEmailAsync(string email, int? excludeId = null)
    {
        var exists = await _context.Owners
            .AnyAsync(o => o.Email == email && (excludeId == null || o.Id != excludeId));
        return ServiceResponse<bool>.Ok(exists);
    }
}