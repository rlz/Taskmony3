namespace Taskmony.Models.Subscriptions;

public class TaskSubscription : Subscription
{
    public Guid TaskId { get; set; }

    public Task Task { get; set; } = default!;
}