namespace Taskmony.Models.Comments;

public class TaskComment : Comment
{
    public Guid TaskId { get; set; }

    public Task Task { get; set; } = default!;
}