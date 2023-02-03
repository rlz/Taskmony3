using Taskmony.Errors;
using Taskmony.Exceptions;
using ValueOf;

namespace Taskmony.ValueObjects;

public class RepeatUntil : ValueOf<DateTime, RepeatUntil>
{
    protected override void Validate()
    {
        if (Value < DateTime.UtcNow)
        {
            throw new DomainException(ValidationErrors.InvalidRepeatUntil);
        }
    }
}
