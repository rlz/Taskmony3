using Taskmony.Errors;
using Taskmony.Exceptions;
using ValueOf;

namespace Taskmony.Models.ValueObjects;

public class DirectionName : ValueOf<string, DirectionName>
{
    protected override void Validate()
    {
        if (string.IsNullOrEmpty(Value) || Value.Length > 120)
        {
            throw new DomainException(ValidationErrors.InvalidDirectionName);
        }
    }
}