using VeterinarySystem.Models;
using VeterinarySystem.Responses;

namespace VeterinarySystem.Interfaces;

public interface IAppointmentService
{
    Task<ServiceResponse<List<Appointment>>> GetAllAsync();
    Task<ServiceResponse<Appointment>> GetByIdAsync(int id);
    Task<ServiceResponse<Appointment>> CreateAsync(Appointment appointment);
    Task<ServiceResponse> CancelAsync(int id);
}