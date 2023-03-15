namespace Taskmony.Models.Subscriptions;

public abstract class Subscription : Entity
{
    public User? User { get; set; }

    public Guid UserId { get; set; }

    public DateTime? CreatedAt { get; set; }
}