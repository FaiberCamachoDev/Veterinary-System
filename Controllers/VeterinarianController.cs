using Microsoft.AspNetCore.Mvc;
using VeterinarySystem.Interfaces;
using VeterinarySystem.Models;

namespace VeterinarySystem.Controllers;

public class VeterinarianController : Controller
{
    private readonly IVeterinarianService _veterinarianService;

    public VeterinarianController(IVeterinarianService veterinarianService)
    {
        _veterinarianService = veterinarianService;
    }

    // GET: Veterinarian
    public async Task<IActionResult> Index()
    {
        var response = await _veterinarianService.GetAllAsync();
        return View(response.Data);
    }

    // GET: Veterinarian/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Veterinarian/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Veterinarian veterinarian)
    {
        // Ignoramos la lista de citas para que no falle la validación del modelo
        ModelState.Remove("Appointments");

        if (ModelState.IsValid)
        {
            var response = await _veterinarianService.CreateAsync(veterinarian);
            if (response.Success)
            {
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction(nameof(Index));
            }
            // Si falla (ej. nombre duplicado), mostramos el error en la vista
            ModelState.AddModelError(string.Empty, response.Message);
        }
        return View(veterinarian);
    }

    // GET: Veterinarian/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _veterinarianService.GetByIdAsync(id);
        if (!response.Success) return NotFound();

        return View(response.Data);
    }

    // POST: Veterinarian/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var response = await _veterinarianService.DeleteAsync(id);
        
        if (response.Success)
        {
            TempData["SuccessMessage"] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        // Si la regla de negocio falla (tiene citas), lo devolvemos a la vista con el error
        TempData["ErrorMessage"] = response.Message;
        return RedirectToAction(nameof(Delete), new { id = id });
    }
}