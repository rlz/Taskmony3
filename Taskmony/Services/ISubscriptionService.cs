namespace Taskmony.Services;

public interface ISubscriptionService
{
    Task<ILookup<Guid, Guid>> GetTaskSubscriberIdsAsync(Guid[] taskIds);
    
    Task<ILookup<Guid, Guid>> GetIdeaSubscriberIdsAsync(Guid[] ideaIds);
    
    Task<bool> SubscribeToTaskAsync(Guid taskId, Guid currentUserId);
    
    Task<bool> SubscribeToIdeaAsync(Guid ideaId, Guid currentUserId);
    
    Task<bool> UnsubscribeFromTaskAsync(Guid taskId, Guid currentUserId);
    
    Task<bool> UnsubscribeFromIdeaAsync(Guid ideaId, Guid currentUserId);
}