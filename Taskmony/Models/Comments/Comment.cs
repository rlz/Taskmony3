using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;
using Taskmony.ValueObjects;

namespace Taskmony.Models.Comments;

public abstract class Comment : IActionItem
{
    public Guid Id { get; set; }

    public ActionItemType ActionItemType => ActionItemType.Comment;

    public CommentText? Text { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid CreatedById { get; set; }

    public User? CreatedBy { get; set; }
}