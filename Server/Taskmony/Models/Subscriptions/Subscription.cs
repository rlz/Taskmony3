using Taskmony.Models.Users;

namespace Taskmony.Models.Subscriptions;

public abstract class Subscription : Entity
{
    public User? User { get; protected set; }

    public Guid UserId { get; protected set; }

    public DateTime? CreatedAt { get; protected set; }

    protected Subscription()
    {
    }

    protected Subscription(Guid userId)
    {
        UserId = userId;
    }
}