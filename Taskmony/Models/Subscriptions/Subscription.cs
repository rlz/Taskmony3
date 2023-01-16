namespace Taskmony.Models.Subscriptions;

public abstract class Subscription
{
    public Guid Id { get; set; }

    public User? User { get; set; }

    public Guid UserId { get; set; }

    public DateTime? SubscribedAt { get; set; }
}