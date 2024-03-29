using Taskmony.Errors;
using Taskmony.Exceptions;
using ValueOf;

namespace Taskmony.Models.ValueObjects;

public class Login : ValueOf<string, Login>
{
    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Value) || Value.Length is < 4 or > 50 || !Value.All(char.IsLetterOrDigit))
        {
            throw new DomainException(ValidationErrors.InvalidLogin);
        }
    }
}