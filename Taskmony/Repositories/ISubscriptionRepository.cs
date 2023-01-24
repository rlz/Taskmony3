using Taskmony.Models.Subscriptions;

namespace Taskmony.Repositories;

public interface ISubscriptionRepository
{
    Task<IEnumerable<TaskSubscription>> GetTaskSubscriptionsAsync(Guid taskId);
    
    Task<IEnumerable<TaskSubscription>> GetTaskSubscriptionsAsync(Guid[] taskIds);

    Task<IEnumerable<IdeaSubscription>> GetIdeaSubscriptionsAsync(Guid ideaId);
    
    Task<IEnumerable<IdeaSubscription>> GetIdeaSubscriptionsAsync(Guid[] ideaIds);
    
    Task<TaskSubscription?> GetTaskSubscriptionAsync(Guid taskId, Guid currentUserId);
    
    Task<IdeaSubscription?> GetIdeaSubscriptionAsync(Guid ideaId, Guid currentUserId);

    void AddTaskSubscription(TaskSubscription subscription);
    
    void AddIdeaSubscription(IdeaSubscription subscription);
    
    void RemoveTaskSubscription(TaskSubscription subscription);
    
    void RemoveIdeaSubscription(IdeaSubscription subscription);

    Task<bool> SaveChangesAsync();
}