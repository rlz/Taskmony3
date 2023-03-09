using Microsoft.Extensions.Options;
using Azure.Communication.Email;
using Azure;

namespace Taskmony.Emails;

public class EmailCommunicationService : IEmailService
{
    private readonly EmailCommunicationSettings _emailSettings;
    private readonly IConfiguration _configuration;

    public EmailCommunicationService(IOptions<EmailCommunicationSettings> emailSettings, IConfiguration configuration)
    {
        _emailSettings = emailSettings.Value;
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string emailTo, string subject, string message)
    {
        var emailClient = new EmailClient(_configuration.GetConnectionString("CommunicationService"));

        var emailContent = new EmailContent(subject)
        {
            Html = message
        };

        await emailClient.SendAsync(WaitUntil.Started, new EmailMessage(
            fromAddress: _emailSettings.SenderEmail, 
            toAddress: emailTo, 
            content: emailContent
        ));
    }
}