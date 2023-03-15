using Taskmony.Errors;

namespace Taskmony.Exceptions;

public class DomainException : Exception
{
    public DomainException(ErrorDetails error) : base(error.Message)
    {
        Error = error;
    }

    public ErrorDetails Error { get; }
}