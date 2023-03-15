using Taskmony.Errors;
using Taskmony.Exceptions;
using ValueOf;

namespace Taskmony.ValueObjects;

public class Description : ValueOf<string, Description>
{
    protected override void Validate()
    {
        if (string.IsNullOrEmpty(Value))
        {
            throw new DomainException(ValidationErrors.InvalidDescription);
        }
    }
}