using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VeterinarySystem.Interfaces;
using VeterinarySystem.Models;

namespace VeterinarySystem.Controllers;

public class PetController : Controller
{
    private readonly IPetService _petService;
    private readonly IOwnerService _ownerService;

    public PetController(IPetService petService, IOwnerService ownerService)
    {
        _petService = petService;
        _ownerService = ownerService;
    }

    // GET: /Pet
    public async Task<IActionResult> Index()
    {
        var response = await _petService.GetAllAsync();
        if (!response.Success)
        {
            TempData["Error"] = response.Message;
            return View(new List<Pet>());
        }
        return View(response.Data);
    }

    // GET: /Pet/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var response = await _petService.GetByIdAsync(id);
        if (!response.Success)
        {
            TempData["Error"] = response.Message;
            return RedirectToAction(nameof(Index));
        }
        return View(response.Data);
    }

    // GET: /Pet/Create?ownerId=3  (opcional, para pre-seleccionar dueño)
    public async Task<IActionResult> Create(int? ownerId)
    {
        await LoadOwnersSelectListAsync(ownerId);
        var pet = new Pet();
        if (ownerId.HasValue) pet.OwnerId = ownerId.Value;
        return View(pet);
    }

    // POST: /Pet/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Pet pet)
    {
        if (!ModelState.IsValid)
        {
            await LoadOwnersSelectListAsync(pet.OwnerId);
            return View(pet);
        }

        var response = await _petService.CreateAsync(pet);
        if (!response.Success)
        {
            foreach (var error in response.Errors)
                ModelState.AddModelError(string.Empty, error);
            await LoadOwnersSelectListAsync(pet.OwnerId);
            return View(pet);
        }

        TempData["Success"] = response.Message;
        return RedirectToAction(nameof(Index));
    }

    // GET: /Pet/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var response = await _petService.GetByIdAsync(id);
        if (!response.Success)
        {
            TempData["Error"] = response.Message;
            return RedirectToAction(nameof(Index));
        }
        await LoadOwnersSelectListAsync(response.Data!.OwnerId);
        return View(response.Data);
    }

    // POST: /Pet/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Pet pet)
    {
        if (!ModelState.IsValid)
        {
            await LoadOwnersSelectListAsync(pet.OwnerId);
            return View(pet);
        }

        var response = await _petService.UpdateAsync(id, pet);
        if (!response.Success)
        {
            foreach (var error in response.Errors)
                ModelState.AddModelError(string.Empty, error);
            await LoadOwnersSelectListAsync(pet.OwnerId);
            return View(pet);
        }

        TempData["Success"] = response.Message;
        return RedirectToAction(nameof(Index));
    }

    // GET: /Pet/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _petService.GetByIdAsync(id);
        if (!response.Success)
        {
            TempData["Error"] = response.Message;
            return RedirectToAction(nameof(Index));
        }
        return View(response.Data);
    }

    // POST: /Pet/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var response = await _petService.DeleteAsync(id);
        if (!response.Success)
        {
            TempData["Error"] = response.Message;
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Success"] = response.Message;
        return RedirectToAction(nameof(Index));
    }

    // Helper privado para cargar el select de propietarios
    private async Task LoadOwnersSelectListAsync(int? selectedId = null)
    {
        var response = await _ownerService.GetAllAsync();
        var owners = response.Data ?? new List<Owner>();
        ViewBag.Owners = new SelectList(owners, "Id", "Name", selectedId);
    }
}