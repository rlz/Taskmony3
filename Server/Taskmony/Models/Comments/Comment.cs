using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;
using Taskmony.ValueObjects;

namespace Taskmony.Models.Comments;

public class Comment : Entity, IActionItem
{
    public ActionItemType ActionItemType => ActionItemType.Comment;

    public CommentText? Text { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DeletedAt? DeletedAt { get; set; }

    public Guid CreatedById { get; set; }

    public User? CreatedBy { get; set; }
}