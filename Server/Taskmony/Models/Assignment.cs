namespace Taskmony.Models;

public class Assignment : Entity
{
    public Guid TaskId { get; set; }

    public Task? Task { get; set; }

    public Guid AssigneeId { get; set; }

    public User Assignee { get; set; } = default!;

    public Guid AssignedById { get; set; }

    public User AssignedBy { get; set; } = default!;
    
    public DateTime? CreatedAt { get; set; }
}