using Taskmony.Errors;
using Taskmony.Exceptions;
using ValueOf;

namespace Taskmony.Models.ValueObjects;

public class CommentText : ValueOf<string, CommentText>
{
    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Value) || Value.Length > 15000)
        {
            throw new DomainException(ValidationErrors.InvalidCommentText);
        }
    }
}