using Taskmony.Errors;
using Taskmony.Exceptions;
using ValueOf;

namespace Taskmony.Models.ValueObjects;

public class ReviewedAt : ValueOf<DateTime, ReviewedAt>
{
    protected override void Validate()
    {
        if (Value > DateTime.UtcNow)
        {
            throw new DomainException(ValidationErrors.InvalidReviewedAt);
        }
    }
}