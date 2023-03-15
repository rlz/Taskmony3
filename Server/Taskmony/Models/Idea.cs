using Taskmony.Models.Comments;
using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;
using Taskmony.Models.Subscriptions;
using Taskmony.ValueObjects;

namespace Taskmony.Models;

public class Idea : DirectionEntity
{
    public override ActionItemType ActionItemType => ActionItemType.Idea;

    public Description? Description { get; set; }

    public string? Details { get; set; }

    public User? CreatedBy { get; set; }

    public Guid CreatedById { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DeletedAt? DeletedAt { get; set; }

    public ReviewedAt? ReviewedAt { get; set; }

    public Generation? Generation { get; set; }

    public ICollection<IdeaComment>? Comments { get; set; }

    public ICollection<Notification>? Notifications { get; set; }

    public ICollection<IdeaSubscription>? Subscriptions { get; set; }
}