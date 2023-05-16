using Taskmony.Models.Ideas;

namespace Taskmony.Models.Subscriptions;

public class IdeaSubscription : Subscription
{
    public Guid IdeaId { get; private set; }

    public Idea Idea { get; private set; } = default!;
    
    // Required by EF Core
    private IdeaSubscription()
    {
    }
    
    public IdeaSubscription(Guid userId, Guid ideaId) : base(userId)
    {
        IdeaId = ideaId;
    }
}