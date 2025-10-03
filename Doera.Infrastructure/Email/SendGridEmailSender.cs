using System.Threading.Tasks;
using Doera.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Doera.Infrastructure.Email;

internal class SendGridEmailSender(
        IConfiguration _config,
        ILogger<SendGridEmailSender> _logger
    ) : IEmailSender
    {
    private readonly string _apiKey = _config["SendGrid:ApiKey"] ?? string.Empty;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            _logger.LogWarning("SendGrid API key missing. Skipping email to {Email}.", email);
            return;
        }

        var client = new SendGridClient(_apiKey);

        var from = new EmailAddress("no-reply@denic.dev", "Doera");
        var to = new EmailAddress(email);

        var msg = MailHelper.CreateSingleEmail(
            from,
            to,
            subject,
            "",
            htmlContent: htmlMessage
        );

        var response = await client.SendEmailAsync(msg);
        if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
        {
            _logger.LogInformation("Email sent to {Email} (Subject: {Subject})", email, subject);
        }
        else
        {
            var body = await response.Body.ReadAsStringAsync();
            _logger.LogError("SendGrid failed ({Status}). To={Email} Subject={Subject} Body={Body}",
                response.StatusCode, email, subject, body);
        }
    }

}