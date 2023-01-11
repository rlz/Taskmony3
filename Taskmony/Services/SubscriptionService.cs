using Taskmony.Models;
using Taskmony.Models.Subscriptions;
using Taskmony.Repositories;

namespace Taskmony.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserService _userService;

    public SubscriptionService(ISubscriptionRepository subscriptionRepository, IUserService userService)
    {
        _subscriptionRepository = subscriptionRepository;
        _userService = userService;
    }

    public async Task<IEnumerable<User>> GetTaskSubscribersAsync(Guid taskId, Guid currentUserId)
    {
        var subscriptions = await _subscriptionRepository.GetTaskSubscriptionsAsync(taskId);

        return await GetSubscribersAsync(subscriptions, currentUserId);
    }

    public async Task<IEnumerable<User>> GetIdeaSubscribersAsync(Guid ideaId, Guid currentUserId)
    {
        var subscriptions = await _subscriptionRepository.GetIdeaSubscriptionsAsync(ideaId);

        return await GetSubscribersAsync(subscriptions, currentUserId);
    }

    private async Task<IEnumerable<User>> GetSubscribersAsync(IEnumerable<Subscription> subscriptions,
        Guid currentUserId)
    {
        var subscribers = subscriptions.Select(s => s.UserId).ToArray();

        return await _userService.GetUsersAsync(subscribers, null, null, null, null, currentUserId);
    }
}