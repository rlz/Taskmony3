using Taskmony.Errors;
using Taskmony.Exceptions;
using ValueOf;

namespace Taskmony.Models.ValueObjects;

public class Details : ValueOf<string?, Details>
{
    protected override void Validate()
    {
        if (Value != null && Value.Length > 16384)
        {
            throw new DomainException(ValidationErrors.InvalidDetails);
        }
    }
}