using Taskmony.Models.Notifications;

namespace Taskmony.Models;

public class Direction
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Details { get; set; }

    public User? CreatedBy { get; set; }

    public Guid CreatedById { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public ICollection<User>? Members { get; set; }

    public ICollection<Task>? Tasks { get; set; }

    public ICollection<Idea>? Ideas { get; set; }

    public ICollection<Notification>? Notifications { get; set; }
}