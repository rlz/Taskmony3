using Taskmony.Models.Users;

namespace Taskmony.Models.Tasks;

public class Assignment : Entity
{
    public Guid TaskId { get; private set; }

    public Task? Task { get; private set; }

    public Guid AssigneeId { get; private set; }

    public User Assignee { get; private set; } = default!;

    public Guid AssignedById { get; private set; }

    public User AssignedBy { get; private set; } = default!;

    public DateTime? CreatedAt { get; private set; }

    // Required by EF Core
    private Assignment()
    {
    }

    public Assignment(Guid taskId, Guid assigneeId, Guid assignedById) : this(assigneeId, assignedById)
    {
        TaskId = taskId;
    }

    public Assignment(Guid assigneeId, Guid assignedById)
    {
        AssigneeId = assigneeId;
        AssignedById = assignedById;
    }
}