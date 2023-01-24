namespace Taskmony.Services;

public interface ISubscriptionService
{
    Task<ILookup<Guid, Guid>> GetTaskSubscriberIdsAsync(Guid[] taskIds);
    
    Task<ILookup<Guid, Guid>> GetIdeaSubscriberIdsAsync(Guid[] ideaIds);
}