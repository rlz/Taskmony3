using System.Text.RegularExpressions;
using Taskmony.Errors;
using Taskmony.Exceptions;
using ValueOf;

namespace Taskmony.ValueObjects;

public class Password : ValueOf<string, Password>
{
    //8+ characters, at least one uppercase letter, one lowercase letter, one number and one special character
    private static readonly Regex EmailRegex =
        new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,100}$", RegexOptions.Compiled);

    protected override void Validate()
    {
        if (!EmailRegex.IsMatch(Value))
        {
            throw new DomainException(ValidationErrors.InvalidPassword);
        }
    }
}