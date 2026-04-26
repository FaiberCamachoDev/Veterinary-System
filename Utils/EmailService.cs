using MailKit.Net.Smtp;
using MimeKit;
using VeterinarySystem.Interfaces;

namespace VeterinarySystem.Utils;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendAppointmentCreatedAsync(string toEmail, string ownerName, 
        string petName, DateTime date, TimeOnly time)
    {
        var subject = "Appointment confirmed.";
        var body = $"""
            Hi {ownerName},

            a medical appointment has been created for {petName}.
            Date: {date:dd/MM/yyyy}
            Hour: {time:HH:mm}

            Please, arrive 10 minutes early.

            Veterinary system
            """;
        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendAppointmentCancelledAsync(string toEmail, string ownerName, 
        string petName, DateTime date)
    {
        var subject = "Medical appointment cancelled";
        var body = $"""
            Hi {ownerName},

            The medical appointment of {date:dd/MM/yyyy} for {petName} has been cancelled.
            If you wish re-schedule, please contact us.

            Veterinary system
            """;
        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendTreatmentAssignedAsync(string toEmail, string ownerName, 
        string petName, string diagnosis)
    {
        var subject = "Treatment assigned";
        var body = $"""
            Hi {ownerName},

            Treatment has been registered for {petName}.
            Diagnosis: {diagnosis}

            Follow the veterinarian instructions.

            Veterinary system
            """;
        await SendEmailAsync(toEmail, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var settings = _config.GetSection("EmailSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                settings["SenderName"], settings["SenderEmail"]));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(
                settings["SmtpHost"],
                int.Parse(settings["SmtpPort"]!),
                bool.Parse(settings["UseSsl"]!));
            await client.AuthenticateAsync(
                settings["SenderEmail"], settings["Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent to {Email}: {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sent email to {Email}", toEmail);
            // No relanzamos: el correo es funcionalidad secundaria, no debe romper el flujo
        }
    }
}