using Microsoft.AspNetCore.Mvc;
using VeterinarySystem.Interfaces;
using VeterinarySystem.Models;

namespace VeterinarySystem.Controllers;

public class OwnerController : Controller
{
    private readonly IOwnerService _ownerService;

    public OwnerController(IOwnerService ownerService)
    {
        _ownerService = ownerService;
    }

    // GET: /Owner
    public async Task<IActionResult> Index()
    {
        var response = await _ownerService.GetAllAsync();
        if (!response.Success)
        {
            TempData["Error"] = response.Message;
            return View(new List<Owner>());
        }
        return View(response.Data);
    }

    // GET: /Owner/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var response = await _ownerService.GetByIdAsync(id);
        if (!response.Success)
        {
            TempData["Error"] = response.Message;
            return RedirectToAction(nameof(Index));
        }
        return View(response.Data);
    }

    // GET: /Owner/Create
    public IActionResult Create() => View();

    // POST: /Owner/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Owner owner)
    {
        if (!ModelState.IsValid)
            return View(owner);

        var response = await _ownerService.CreateAsync(owner);
        if (!response.Success)
        {
            foreach (var error in response.Errors)
                ModelState.AddModelError(string.Empty, error);
            return View(owner);
        }

        TempData["Success"] = response.Message;
        return RedirectToAction(nameof(Index));
    }

    // GET: /Owner/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var response = await _ownerService.GetByIdAsync(id);
        if (!response.Success)
        {
            TempData["Error"] = response.Message;
            return RedirectToAction(nameof(Index));
        }
        return View(response.Data);
    }

    // POST: /Owner/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Owner owner)
    {
        if (!ModelState.IsValid)
            return View(owner);

        var response = await _ownerService.UpdateAsync(id, owner);
        if (!response.Success)
        {
            foreach (var error in response.Errors)
                ModelState.AddModelError(string.Empty, error);
            return View(owner);
        }

        TempData["Success"] = response.Message;
        return RedirectToAction(nameof(Index));
    }

    // GET: /Owner/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _ownerService.GetByIdAsync(id);
        if (!response.Success)
        {
            TempData["Error"] = response.Message;
            return RedirectToAction(nameof(Index));
        }
        return View(response.Data);
    }

    // POST: /Owner/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var response = await _ownerService.DeleteAsync(id);
        if (!response.Success)
        {
            TempData["Error"] = response.Message;
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Success"] = response.Message;
        return RedirectToAction(nameof(Index));
    }
}