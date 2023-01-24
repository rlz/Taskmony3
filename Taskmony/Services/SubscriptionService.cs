using Taskmony.Repositories;

namespace Taskmony.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public SubscriptionService(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<ILookup<Guid, Guid>> GetTaskSubscriberIdsAsync(Guid[] taskIds)
    {
        var subscriptions = await _subscriptionRepository.GetTaskSubscriptionsAsync(taskIds);
        
        return subscriptions.ToLookup(s => s.TaskId, s => s.UserId);
    }

    public async Task<ILookup<Guid, Guid>> GetIdeaSubscriberIdsAsync(Guid[] ideaIds)
    {
        var subscriptions = await _subscriptionRepository.GetIdeaSubscriptionsAsync(ideaIds);
        
        return subscriptions.ToLookup(s => s.IdeaId, s => s.UserId);
    }
}