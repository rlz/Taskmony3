using Microsoft.Extensions.Options;
using Azure.Communication.Email;
using Azure;

namespace Taskmony.Emails;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly IConfiguration _configuration;

    public EmailService(IOptions<EmailSettings> emailSettings, IConfiguration configuration)
    {
        _emailSettings = emailSettings.Value;
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string emailTo, string subject, string message)
    {
        var emailClient = new EmailClient(_configuration.GetConnectionString("CommunicationService"));

        EmailContent emailContent = new EmailContent(subject);
        emailContent.PlainText = message;

        EmailMessage emailMessage = new EmailMessage(_emailSettings.SenderEmail, emailTo, emailContent);
        var emailSendOperation = await emailClient.SendAsync(WaitUntil.Completed, emailMessage);
    }
}