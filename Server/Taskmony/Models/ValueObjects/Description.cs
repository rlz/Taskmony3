using Taskmony.Errors;
using Taskmony.Exceptions;
using ValueOf;

namespace Taskmony.Models.ValueObjects;

public class Description : ValueOf<string, Description>
{
    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Value) || Value.Length > 500)
        {
            throw new DomainException(ValidationErrors.InvalidDescription);
        }
    }
}