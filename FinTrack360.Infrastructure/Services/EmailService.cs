using FinTrack360.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Net;
using System.Net.Mail;

namespace FinTrack360.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly string _host;
    private readonly int _port;
    private readonly bool _enableSsl;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly string _username;
    private readonly string _password;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _host = _configuration["SmtpSettings:Host"] ?? throw new("SmtpSettings:Host");
        _port = _configuration.GetValue<int>("SmtpSettings:Port");
        _enableSsl = _configuration.GetValue<bool>("SmtpSettings:EnableSsl");
        _fromEmail = _configuration["SmtpSettings:FromEmail"] ?? throw new("SmtpSettings:FromEmail");
        _fromName = _configuration["SmtpSettings:FromName"] ?? "FinTrack360";
        _username = _configuration["SmtpSettings:Username"] ?? throw new("SmtpSettings:Username");
        _password = _configuration["SmtpSettings:Password"] ?? throw new("SmtpSettings:Password");
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string firstName, string token, string language)
    {
        var culture = GetNormalizedCulture(language);
        var templateName = $"PasswordResetTemplate.{culture}.html";

        string template;
        try
        {
            template = await ReadTemplateAsync(templateName);
        }
        catch (FileNotFoundException)
        {
            template = await ReadTemplateAsync("PasswordResetTemplate.en-US.html");
            culture = "en-US";
        }

        var frontendUrl = _configuration.GetValue<string>("ClientAppUrl") ?? "http://localhost:3000";
        var resetLink = $"{frontendUrl}/reset-password?token={token}&email={toEmail}";

        template = template.Replace("[UserName]", firstName);
        template = template.Replace("[ResetLink]", resetLink);

        var subject = culture.StartsWith("pt") ? "Seu link de redefinição de senha" : "Your password reset link";
        await SendEmailAsync(toEmail, subject, template);
    }

    public async Task SendConfirmationEmailAsync(string toEmail, string firstName, string token, string language)
    {
        var culture = GetNormalizedCulture(language);
        var templateName = $"ConfirmEmailTemplate.{culture}.html";

        string template;
        try
        {
            template = await ReadTemplateAsync(templateName);
        }
        catch (FileNotFoundException)
        {
            template = await ReadTemplateAsync("ConfirmEmailTemplate.en-US.html");
            culture = "en-US";
        }

        var apiUrl = _configuration.GetValue<string>("ApiUrl") ?? "https://localhost:7241";
        var confirmationLink = $"{apiUrl}/api/auth/confirm-email?token={token}&email={toEmail}&lang={culture}";

        template = template.Replace("[UserName]", firstName);
        template = template.Replace("[ConfirmationLink]", confirmationLink);

        var subject = culture.StartsWith("pt") ? "Confirme seu e-mail - FinTrack360" : "Confirm your email - FinTrack360";
        await SendEmailAsync(toEmail, subject, template);
    }

    public async Task<string> GetEmailConfirmationPageAsync(string language)
    {
        var culture = GetNormalizedCulture(language);
        var templateName = $"EmailConfirmedTemplate.{culture}.html";

        try
        {
            return await ReadTemplateAsync(templateName);
        }
        catch (FileNotFoundException)
        {
            return await ReadTemplateAsync("EmailConfirmedTemplate.en-US.html");
        }
    }

    private async Task SendEmailAsync(string to, string subject, string htmlContent)
    {
        using var mailMessage = new MailMessage
        {
            From = new MailAddress(_fromEmail, _fromName),
            Subject = subject,
            Body = htmlContent,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(to);

        using var smtpClient = new SmtpClient(_host, _port)
        {
            Credentials = new NetworkCredential(_username, _password),
            EnableSsl = _enableSsl
        };

        await smtpClient.SendMailAsync(mailMessage);
    }

    private static async Task<string> ReadTemplateAsync(string templateName)
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var templatePath = Path.Combine(basePath, "EmailTemplates", templateName);

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template file '{templateName}' not found at path '{templatePath}'.");
        }

        return await File.ReadAllTextAsync(templatePath);
    }

    private static string GetNormalizedCulture(string culture)
    {
        if (string.IsNullOrEmpty(culture)) return "en-US";

        var parts = culture.Split('-');
        if (parts.Length == 1) return culture;

        return $"{parts[0].ToLowerInvariant()}-{parts[1].ToUpperInvariant()}";
    }
}