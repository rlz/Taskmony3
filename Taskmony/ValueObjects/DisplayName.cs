using Taskmony.Errors;
using Taskmony.Exceptions;
using ValueOf;

namespace Taskmony.ValueObjects;

public class DisplayName : ValueOf<string, DisplayName>
{
    protected override void Validate()
    {
        if (string.IsNullOrEmpty(Value) || Value.Length is < 3 or > 50)
        {
            throw new DomainException(ValidationErrors.InvalidDisplayName);
        }
    }
}