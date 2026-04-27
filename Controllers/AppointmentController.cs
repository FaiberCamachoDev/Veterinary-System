using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VeterinarySystem.Data;
using VeterinarySystem.Interfaces;
using VeterinarySystem.Models;

namespace VeterinarySystem.Controllers;

public class AppointmentController : Controller
{
    private readonly IAppointmentService _appointmentService;
    private readonly VeterinaryContext _context; 

    public AppointmentController(IAppointmentService appointmentService, VeterinaryContext context)
    {
        _appointmentService = appointmentService;
        _context = context;
    }

    // GET: Appointment
    public async Task<IActionResult> Index()
    {
        var response = await _appointmentService.GetAllAsync();
        return View(response.Data);
    }

    // GET: Appointment/Create
    public IActionResult Create()
    {
        CargarListasDesplegables();
        return View();
    }

    // POST: Appointment/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Appointment appointment)
    {
        // 1. Ignoramos las propiedades de navegación para la validación.
        ModelState.Remove("Owner");
        ModelState.Remove("Pet");
        ModelState.Remove("Veterinarian");
        ModelState.Remove("Treatment");

        if (ModelState.IsValid)
        {
            var response = await _appointmentService.CreateAsync(appointment);
            
            if (response.Success)
            {
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction(nameof(Index));
            }
            
            // Si la lógica de negocio falla (ej. solapamiento, veterinario ocupado, cliente bloqueado)
            ModelState.AddModelError(string.Empty, response.Message);
        }

        // Si hay error de validación, recargamos las listas para que los <select> no queden vacíos
        CargarListasDesplegables(appointment);
        return View(appointment);
    }

    // GET: Appointment/Cancel/5
    public async Task<IActionResult> Cancel(int id)
    {
        var response = await _appointmentService.GetByIdAsync(id);
        if (!response.Success) return NotFound();

        return View(response.Data);
    }

    // POST: Appointment/Cancel/5
    [HttpPost, ActionName("Cancel")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelConfirmed(int id)
    {
        var response = await _appointmentService.CancelAsync(id);
        
        if (response.Success)
        {
            TempData["SuccessMessage"] = response.Message;
        }
        else
        {
            TempData["ErrorMessage"] = response.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    // --- Método Auxiliar Privado ---
    // Mantiene el código DRY al cargar los Selects
    private void CargarListasDesplegables(Appointment? appointment = null)
    {
        ViewBag.Owners = new SelectList(_context.Owners, "Id", "Name", appointment?.OwnerId);
        ViewBag.Pets = new SelectList(_context.Pets, "Id", "Name", appointment?.PetId);
        
        // Para el veterinario, creamos un nombre compuesto más amigable para el Select
        var vets = _context.Veterinarians.Select(v => new 
        {
            Id = v.Id,
            DisplayName = $"{v.Name} - {v.Specialty}"
        }).ToList();
        
        ViewBag.Veterinarians = new SelectList(vets, "Id", "DisplayName", appointment?.VeterinarianId);
    }
}