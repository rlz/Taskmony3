using Taskmony.Models.Notifications;
using Taskmony.ValueObjects;

namespace Taskmony.Models;

public class Direction : Entity
{
    public DirectionName? Name { get; set; }

    public string? Details { get; set; }

    public User? CreatedBy { get; set; }

    public Guid CreatedById { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DeletedAt? DeletedAt { get; set; }

    public ICollection<User>? Members { get; set; }

    public ICollection<Task>? Tasks { get; set; }

    public ICollection<Idea>? Ideas { get; set; }

    public ICollection<Notification>? Notifications { get; set; }
}