using Invoice.Domain.Configurations;
using Invoice.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace Invoice.Application.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(EmailSettings emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            if (string.IsNullOrEmpty(to))
            {
                _logger.LogWarning("Recipient email is empty");
                return false;
            }

            using (var client = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderDisplayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);

                _logger.LogInformation($"Email sent successfully to {to}");
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending email to {to}");
            return false;
        }
    }

    public async Task<bool> SendPasswordResetEmailAsync(string to, string userName, string resetLink)
    {
        try
        {
            var subject = "Password Reset Request";
            var body = $@"
                <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2>Password Reset Request</h2>
                        <p>Hello {userName},</p>
                        <p>We received a request to reset your password. Click the link below to reset it:</p>
                        <p>
                            <a href='{resetLink}' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 4px; display: inline-block;'>
                                Reset Password
                            </a>
                        </p>
                        <p>This link will expire in 1 hour.</p>
                        <p>If you didn't request this, please ignore this email.</p>
                        <p>Best regards,<br/>Invoice System Team</p>
                    </body>
                </html>";

            return await SendEmailAsync(to, subject, body, true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending password reset email to {to}");
            return false;
        }
    }
}
