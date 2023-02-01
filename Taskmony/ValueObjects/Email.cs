using Taskmony.Errors;
using Taskmony.Exceptions;
using ValueOf;

namespace Taskmony.ValueObjects;

public class Email : ValueOf<string, Email>
{
    protected override void Validate()
    {   
        try
        {
            _ = new System.Net.Mail.MailAddress(Value);
        }
        catch
        {
            throw new DomainException(ValidationErrors.InvalidEmail);
        }
    }
}