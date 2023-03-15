namespace Taskmony.Models.Subscriptions;

public class IdeaSubscription : Subscription
{
    public Guid IdeaId { get; set; }

    public Idea Idea { get; set; } = default!;
}