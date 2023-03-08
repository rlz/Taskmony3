namespace Taskmony.Emails;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string message);
}