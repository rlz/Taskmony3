using Taskmony.ValueObjects;
using Task = Taskmony.Models.Tasks.Task;

namespace Taskmony.Models.Comments;

public class TaskComment : Comment
{
    public Guid TaskId { get; private set; }

    public Task Task { get; private set; } = default!;

    // Required by EF Core
    private TaskComment()
    {
    }

    public TaskComment(CommentText text, Guid createdById, Guid taskId, DateTime? createdAt = null,
        DeletedAt? deletedAt = null) : base(text, createdById, createdAt, deletedAt)
    {
        TaskId = taskId;
    }
}