using Taskmony.Models.Subscriptions;

namespace Taskmony.Repositories.Abstract;

public interface ISubscriptionRepository
{
    Task<IEnumerable<TaskSubscription>> GetByTaskIdsAsync(Guid[] taskIds, int? offset, int? limit);
    
    Task<IEnumerable<IdeaSubscription>> GetByIdeaIdsAsync(Guid[] ideaIds, int? offset, int? limit);
    
    Task<TaskSubscription?> GetByTaskAndUserAsync(Guid taskId, Guid currentUserId);
    
    Task<IdeaSubscription?> GetByIdeaAndUserAsync(Guid ideaId, Guid currentUserId);
    
    void Add(Subscription subscription);
    
    void Delete(Subscription subscription);

    Task<bool> SaveChangesAsync();
}