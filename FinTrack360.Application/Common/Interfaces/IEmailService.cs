namespace FinTrack360.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string firstName, string token, string language);
    Task SendConfirmationEmailAsync(string toEmail, string firstName, string token, string language);
    Task<string> GetEmailConfirmationPageAsync(string language);
}