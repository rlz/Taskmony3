using Taskmony.Errors;
using Taskmony.Exceptions;
using ValueOf;

namespace Taskmony.Models.ValueObjects;

public class DeletedAt : ValueOf<DateTime, DeletedAt>
{
    protected override void Validate()
    {
        if (Value > DateTime.UtcNow)
        {
            throw new DomainException(ValidationErrors.InvalidDeletedAt);
        }
    }
}