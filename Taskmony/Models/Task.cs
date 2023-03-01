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

    public User? Assignee { get; set; }

    public Guid? AssigneeId { get; set; }

    public User? AssignedBy { get; set; }

    public Guid? AssignedById { get; set; }

    public RepeatMode? RepeatMode { get; set; }

    public WeekDay? WeekDays { get; set; }

    public int? RepeatEvery { get; set; }

    public DateTime? RepeatUntil { get; set; }
    
    public Guid? GroupId { get; set; }

    public ICollection<TaskComment>? Comments { get; set; }

    public ICollection<Notification>? Notifications { get; set; }

    public ICollection<TaskSubscription>? Subscriptions { get; set; }
}