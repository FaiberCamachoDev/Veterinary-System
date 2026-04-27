using Microsoft.EntityFrameworkCore;
using VeterinarySystem.Data;
using VeterinarySystem.Interfaces;
using VeterinarySystem.Models;
using VeterinarySystem.Responses;

namespace VeterinarySystem.Services;

public class PetService : IPetService
{
    private readonly VeterinaryContext _context;

    public PetService(VeterinaryContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<List<Pet>>> GetAllAsync()
    {
        try
        {
            var pets = await _context.Pets
                .Include(p => p.Owner)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return ServiceResponse<List<Pet>>.Ok(pets);
        }
        catch (Exception ex)
        {
            return ServiceResponse<List<Pet>>.Fail($"Error al obtener mascotas: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<List<Pet>>> GetByOwnerAsync(int ownerId)
    {
        try
        {
            var ownerExists = await _context.Owners.AnyAsync(o => o.Id == ownerId);
            if (!ownerExists)
                return ServiceResponse<List<Pet>>.Fail("Propietario no encontrado.");

            var pets = await _context.Pets
                .Include(p => p.Owner)
                .Where(p => p.OwnerId == ownerId)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return ServiceResponse<List<Pet>>.Ok(pets);
        }
        catch (Exception ex)
        {
            return ServiceResponse<List<Pet>>.Fail($"Error al obtener mascotas: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<Pet>> GetByIdAsync(int id)
    {
        try
        {
            var pet = await _context.Pets
                .Include(p => p.Owner)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Veterinarian)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pet is null)
                return ServiceResponse<Pet>.Fail("Mascota no encontrada.");

            return ServiceResponse<Pet>.Ok(pet);
        }
        catch (Exception ex)
        {
            return ServiceResponse<Pet>.Fail($"Error al obtener mascota: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<Pet>> CreateAsync(Pet pet)
    {
        try
        {
            var ownerExists = await _context.Owners.AnyAsync(o => o.Id == pet.OwnerId);
            if (!ownerExists)
                return ServiceResponse<Pet>.Fail("El propietario especificado no existe.");

            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            return ServiceResponse<Pet>.Ok(pet, "Mascota registrada exitosamente.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<Pet>.Fail($"Error al crear mascota: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<Pet>> UpdateAsync(int id, Pet pet)
    {
        try
        {
            var existing = await _context.Pets.FindAsync(id);

            if (existing is null)
                return ServiceResponse<Pet>.Fail("Mascota no encontrada.");

            var ownerExists = await _context.Owners.AnyAsync(o => o.Id == pet.OwnerId);
            if (!ownerExists)
                return ServiceResponse<Pet>.Fail("El propietario especificado no existe.");

            existing.Name = pet.Name;
            existing.Species = pet.Species;
            existing.Breed = pet.Breed;
            existing.Age = pet.Age;
            existing.Weight = pet.Weight;
            existing.OwnerId = pet.OwnerId;

            await _context.SaveChangesAsync();

            return ServiceResponse<Pet>.Ok(existing, "Mascota actualizada exitosamente.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<Pet>.Fail($"Error al actualizar mascota: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(int id)
    {
        try
        {
            var pet = await _context.Pets
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pet is null)
                return ServiceResponse<bool>.Fail("Mascota no encontrada.");

            if (pet.Appointments.Count > 0)
                return ServiceResponse<bool>.Fail(
                    "No se puede eliminar la mascota porque tiene citas registradas.");

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();

            return ServiceResponse<bool>.Ok(true, "Mascota eliminada exitosamente.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Fail($"Error al eliminar mascota: {ex.Message}");
        }
    }
}