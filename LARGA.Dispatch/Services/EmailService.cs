using System.Net;
using System.Net.Mail;

namespace LARGA.Dispatch.Services;

/// <summary>
/// Integration 5 - Email through SMTP (Gmail by default). Settings live in the "Email" section of
/// appsettings.Local.json (see appsettings.Local.json.example). With no settings it only logs the
/// message, so the app still works without email set up.
/// </summary>
public class EmailService(IConfiguration config, ILogger<EmailService> logger)
{
    /// <summary>Returns a short status message that the UI can show to the dispatcher.</summary>
    public async Task<string> SendAsync(string to, string subject, string body)
    {
        string? username = config["Email:Username"];
        string? password = config["Email:Password"];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogInformation("Email not sent (SMTP not configured) to {To}: {Subject}\n{Body}", to, subject, body);
            return $"Email to {to} was logged only (SMTP not configured).";
        }

        try
        {
            using SmtpClient client = new(config["Email:Host"] ?? "smtp.gmail.com", config.GetValue("Email:Port", 587))
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(username, password)
            };
            using MailMessage message = new(new MailAddress(username, "LARGA Dispatch"), new MailAddress(to))
            {
                Subject = subject,
                Body = body
            };
            await client.SendMailAsync(message);
            return $"Email sent to {to}.";
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Email to {To} failed", to);
            return $"Email to {to} failed: {ex.Message}";
        }
    }
}
