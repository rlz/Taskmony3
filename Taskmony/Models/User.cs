using Taskmony.Models.Comments;
using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;
using Taskmony.Models.Subscriptions;
using Taskmony.ValueObjects;

namespace Taskmony.Models;

public class User : Entity, IActionItem
{
    public ActionItemType ActionItemType => ActionItemType.User;

    public Login? Login { get; set; }

    public string? Password { get; set; }

    public DisplayName? DisplayName { get; set; }

    public Email? Email { get; set; }

    public DateTime? NotificationReadTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Tasks created by the user
    /// </summary>
    public ICollection<Task>? Tasks { get; set; }

    /// <summary>
    /// Tasks assigned to the user
    /// </summary>
    public ICollection<Task>? AssignedTasks { get; set; }

    public ICollection<Idea>? Ideas { get; set; }

    /// <summary>
    /// Directions in which the user is a member
    /// </summary>
    public ICollection<Direction>? Directions { get; set; }

    /// <summary>
    /// Directions owned by the user
    /// </summary>
    public ICollection<Direction>? OwnDirections { get; set; }

    public ICollection<Subscription>? Subscriptions { get; set; }

    public ICollection<Comment>? Comments { get; set; }
}