using Taskmony.Errors;
using Taskmony.Exceptions;
using ValueOf;

namespace Taskmony.Models.ValueObjects;

public class CompletedAt : ValueOf<DateTime, CompletedAt>
{
    protected override void Validate()
    {
        if (Value > DateTime.UtcNow)
        {
            throw new DomainException(ValidationErrors.InvalidCompletedAt);
        }
    }
}