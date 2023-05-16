using Task = Taskmony.Models.Tasks.Task;

namespace Taskmony.Models.Subscriptions;

public class TaskSubscription : Subscription
{
    public Guid TaskId { get; private set; }

    public Task Task { get; private set; } = default!;
    
    // Required by EF Core
    private TaskSubscription()
    {
    }
    
    public TaskSubscription(Guid userId, Guid taskId) : base(userId)
    {
        TaskId = taskId;
    }
}