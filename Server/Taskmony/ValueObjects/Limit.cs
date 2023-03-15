using Taskmony.Errors;
using Taskmony.Exceptions;
using ValueOf;

namespace Taskmony.ValueObjects;

public class Limit : ValueOf<int, Limit>
{
    protected override void Validate()
    {
        if (Value < 0)
        {
            throw new DomainException(ValidationErrors.InvalidLimit);
        }
    }
}