using Taskmony.Models.Subscriptions;

namespace Taskmony.Repositories;

public interface ISubscriptionRepository
{
    Task<IEnumerable<TaskSubscription>> GetTaskSubscriptionsAsync(Guid taskId);
    
    Task<IEnumerable<TaskSubscription>> GetTaskSubscriptionsAsync(Guid[] taskIds);

    Task<IEnumerable<IdeaSubscription>> GetIdeaSubscriptionsAsync(Guid ideaId);
    
    Task<IEnumerable<IdeaSubscription>> GetIdeaSubscriptionsAsync(Guid[] ideaIds);
    
    Task<bool> SaveChangesAsync();
}