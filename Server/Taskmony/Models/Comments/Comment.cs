using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Notifications;
using Taskmony.Models.Users;
using Taskmony.ValueObjects;

namespace Taskmony.Models.Comments;

public class Comment : Entity, IActionItem
{
    public ActionItemType ActionItemType => ActionItemType.Comment;

    public CommentText? Text { get; protected set; }

    public DateTime? CreatedAt { get; protected set; }

    public DeletedAt? DeletedAt { get; protected set; }

    public Guid CreatedById { get; private set; }

    public User? CreatedBy { get; protected set; }

    protected Comment()
    {
    }

    protected Comment(CommentText text, Guid createdById, DateTime? createdAt = null, DeletedAt? deletedAt = null)
    {
        Text = text;
        CreatedById = createdById;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        DeletedAt = deletedAt;
    }

    public Comment(Guid id, CommentText text, Guid createdById, DateTime? createdAt = null, DeletedAt? deletedAt = null)
    {
        Id = id;
        Text = text;
        CreatedById = createdById;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        DeletedAt = deletedAt;
    }

    public void UpdateText(CommentText text)
    {
        Text = text;
    }

    public void UpdateDeletedAt(DeletedAt? deletedAt)
    {
        if (deletedAt != null && DeletedAt != null)
        {
            throw new DomainException(CommentErrors.AlreadyDeleted);
        }

        DeletedAt = deletedAt;
    }
}