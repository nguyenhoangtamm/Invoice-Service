namespace Invoice.Domain.Interfaces.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task<bool> SendPasswordResetEmailAsync(string to, string userName, string resetLink);
}
