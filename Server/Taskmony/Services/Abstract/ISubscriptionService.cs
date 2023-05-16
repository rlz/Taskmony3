namespace Taskmony.Services.Abstract;

public interface ISubscriptionService
{
    Task<ILookup<Guid, Guid>> GetTaskSubscriberIdsAsync(Guid[] taskIds, int? offset, int? limit);

    Task<ILookup<Guid, Guid>> GetIdeaSubscriberIdsAsync(Guid[] ideaIds, int? offset, int? limit);

    Task<bool> SubscribeToTaskAsync(Guid taskId, Guid currentUserId);

    Task<bool> SubscribeToIdeaAsync(Guid ideaId, Guid currentUserId);

    Task<bool> UnsubscribeFromTaskAsync(Guid taskId, Guid currentUserId);

    Task<bool> UnsubscribeFromIdeaAsync(Guid ideaId, Guid currentUserId);
}