using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Subscriptions;
using Taskmony.Models.ValueObjects;
using Taskmony.Repositories.Abstract;
using Taskmony.Services.Abstract;

namespace Taskmony.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ITaskService _taskService;
    private readonly IIdeaService _ideaService;

    public SubscriptionService(ISubscriptionRepository subscriptionRepository, ITaskService taskService,
        IIdeaService ideaService)
    {
        _subscriptionRepository = subscriptionRepository;
        _taskService = taskService;
        _ideaService = ideaService;
    }

    public async Task<ILookup<Guid, Guid>> GetTaskSubscriberIdsAsync(Guid[] taskIds, int? offset, int? limit)
    {
        int? limitValue = limit is null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset is null ? null : Offset.From(offset.Value).Value;

        var subscriptions = await _subscriptionRepository.GetByTaskIdsAsync(taskIds, offsetValue, limitValue);

        return subscriptions.ToLookup(s => s.TaskId, s => s.UserId);
    }

    public async Task<ILookup<Guid, Guid>> GetIdeaSubscriberIdsAsync(Guid[] ideaIds, int? offset, int? limit)
    {
        int? limitValue = limit is null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset is null ? null : Offset.From(offset.Value).Value;

        var subscriptions = await _subscriptionRepository.GetByIdeaIdsAsync(ideaIds, offsetValue, limitValue);

        return subscriptions.ToLookup(s => s.IdeaId, s => s.UserId);
    }

    public async Task<bool> SubscribeToTaskAsync(Guid taskId, Guid currentUserId)
    {
        var task = await _taskService.GetTaskOrThrowAsync(taskId, currentUserId);

        if (task.DeletedAt is not null)
        {
            throw new DomainException(SubscriptionErrors.SubscribeToDeletedEntity);
        }

        if (task.CompletedAt is not null)
        {
            throw new DomainException(SubscriptionErrors.SubscribeToCompletedTask);
        }

        var subscription = await _subscriptionRepository.GetByTaskAndUserAsync(taskId, currentUserId);

        if (subscription is not null)
        {
            throw new DomainException(SubscriptionErrors.AlreadySubscribed);
        }

        _subscriptionRepository.Add(new TaskSubscription(currentUserId, taskId));

        return await _subscriptionRepository.SaveChangesAsync();
    }

    public async Task<bool> SubscribeToIdeaAsync(Guid ideaId, Guid currentUserId)
    {
        var idea = await _ideaService.GetIdeaOrThrowAsync(ideaId, currentUserId);

        if (idea.DeletedAt is not null)
        {
            throw new DomainException(SubscriptionErrors.SubscribeToDeletedEntity);
        }

        var subscription = await _subscriptionRepository.GetByIdeaAndUserAsync(ideaId, currentUserId);

        if (subscription is not null)
        {
            throw new DomainException(SubscriptionErrors.AlreadySubscribed);
        }

        _subscriptionRepository.Add(new IdeaSubscription(currentUserId, ideaId));

        return await _subscriptionRepository.SaveChangesAsync();
    }

    public async Task<bool> UnsubscribeFromTaskAsync(Guid taskId, Guid currentUserId)
    {
        var taskSubscription = await _subscriptionRepository.GetByTaskAndUserAsync(taskId, currentUserId);

        return await UnsubscribeAsync(taskSubscription);
    }

    public async Task<bool> UnsubscribeFromIdeaAsync(Guid ideaId, Guid currentUserId)
    {
        var ideaSubscription = await _subscriptionRepository.GetByIdeaAndUserAsync(ideaId, currentUserId);

        return await UnsubscribeAsync(ideaSubscription);
    }

    private async Task<bool> UnsubscribeAsync(Subscription? subscription)
    {
        if (subscription is null)
        {
            throw new DomainException(SubscriptionErrors.NotFound);
        }

        _subscriptionRepository.Delete(subscription);

        return await _subscriptionRepository.SaveChangesAsync();
    }
}