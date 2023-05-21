using System.Text.RegularExpressions;
using Taskmony.Errors;
using Taskmony.Exceptions;
using ValueOf;

namespace Taskmony.Models.ValueObjects;

public class Password : ValueOf<string, Password>
{
    //8+ characters, at least one uppercase letter, one lowercase letter, one number and optionally special characters
    private static readonly Regex EmailRegex =
            new("""^(?=\D*\d)(?=[^a-z]*[a-z])(?=[^A-Z]*[A-Z])[\w~@#$%^&*+=`|{}:;!.?"()\[\]-]{8,100}$""", RegexOptions.Compiled);

    protected override void Validate()
    {
        if (Value == null || !EmailRegex.IsMatch(Value))
        {
            throw new DomainException(ValidationErrors.InvalidPassword);
        }
    }
}