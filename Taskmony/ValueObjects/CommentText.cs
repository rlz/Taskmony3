using Taskmony.Errors;
using Taskmony.Exceptions;
using ValueOf;

namespace Taskmony.ValueObjects;

public class CommentText : ValueOf<string, CommentText>
{
    protected override void Validate()
    {
        if (string.IsNullOrEmpty(Value))
        {
            throw new DomainException(ValidationErrors.InvalidCommentText);
        }
    }
}