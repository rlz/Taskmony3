using Taskmony.Models.Comments;
using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;
using Taskmony.Models.Subscriptions;
using Taskmony.ValueObjects;

namespace Taskmony.Models;

public class Task : DirectionEntity
{
    public override ActionItemType ActionItemType => ActionItemType.Task;

    public Description? Description { get; set; }

    public string? Details { get; set; }

    public User? CreatedBy { get; set; }

    public Guid CreatedById { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? StartAt { get; set; }

    public CompletedAt? CompletedAt { get; set; }

    public DeletedAt? DeletedAt { get; set; }

    public Assignment? Assignment { get; set; }

    public RecurrencePattern? RecurrencePattern { get; set; }
    
    public Guid? GroupId { get; set; }

    public ICollection<TaskComment>? Comments { get; set; }

    public ICollection<Notification>? Notifications { get; set; }

    public ICollection<TaskSubscription>? Subscriptions { get; set; }
}