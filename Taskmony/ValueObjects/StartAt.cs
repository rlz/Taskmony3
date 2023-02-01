using Taskmony.Errors;
using Taskmony.Exceptions;
using ValueOf;

namespace Taskmony.ValueObjects;

public class StartAt : ValueOf<DateTime, StartAt>
{
    protected override void Validate()
    {
        if (Value > DateTime.UtcNow)
        {
            throw new DomainException(ValidationErrors.InvalidCompletedAt);
        }
    }
}