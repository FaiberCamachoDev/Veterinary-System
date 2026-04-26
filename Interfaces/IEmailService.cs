namespace VeterinarySystem.Interfaces;

public interface IEmailService
{
    Task SendAppointmentCreatedAsync(string toEmail, string ownerName, 
        string petName, DateTime date, TimeOnly time);
    
    Task SendAppointmentCancelledAsync(string toEmail, string ownerName, 
        string petName, DateTime date);
    
    Task SendTreatmentAssignedAsync(string toEmail, string ownerName, 
        string petName, string diagnosis);
}