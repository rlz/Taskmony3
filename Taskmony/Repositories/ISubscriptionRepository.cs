using Taskmony.Models.Subscriptions;

namespace Taskmony.Repositories;

public interface ISubscriptionRepository
{
    Task<IEnumerable<TaskSubscription>> GetTaskSubscriptionsAsync(Guid taskId);

    Task<IEnumerable<IdeaSubscription>> GetIdeaSubscriptionsAsync(Guid ideaId);
    
    Task<bool> SaveChangesAsync();
}