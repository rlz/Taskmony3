using Taskmony.Services.Abstract;

namespace Taskmony.GraphQL.Subscriptions;

[ExtendObjectType(typeof(Mutation))]
public class SubscriptionMutations
{
    public async Task<Guid?> TaskSubscribe([Service] ISubscriptionService subscriptionService,
        [GlobalState] Guid currentUserId, Guid taskId)
    {
        if (await subscriptionService.SubscribeToTaskAsync(taskId, currentUserId))
        {
            return taskId;
        }

        return null;
    }

    public async Task<Guid?> IdeaSubscribe([Service] ISubscriptionService subscriptionService,
        [GlobalState] Guid currentUserId, Guid ideaId)
    {
        if (await subscriptionService.SubscribeToIdeaAsync(ideaId, currentUserId))
        {
            return ideaId;
        }

        return null;
    }

    public async Task<Guid?> TaskUnsubscribe([Service] ISubscriptionService subscriptionService,
        [GlobalState] Guid currentUserId, Guid taskId)
    {
        if (await subscriptionService.UnsubscribeFromTaskAsync(taskId, currentUserId))
        {
            return taskId;
        }

        return null;
    }

    public async Task<Guid?> IdeaUnsubscribe([Service] ISubscriptionService subscriptionService,
        [GlobalState] Guid currentUserId, Guid ideaId)
    {
        if (await subscriptionService.UnsubscribeFromIdeaAsync(ideaId, currentUserId))
        {
            return ideaId;
        }

        return null;
    }
}