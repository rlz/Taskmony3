using Taskmony.Errors;
using Taskmony.Exceptions;
using ValueOf;

namespace Taskmony.Models.ValueObjects;

public class Offset : ValueOf<int, Offset>
{
    protected override void Validate()
    {
        if (Value < 0)
        {
            throw new DomainException(ValidationErrors.InvalidOffset);
        }
    }
}