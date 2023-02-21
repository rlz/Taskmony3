using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Subscriptions;
using Taskmony.Repositories.Abstract;
using Taskmony.Services.Abstract;
using Taskmony.ValueObjects;

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
            throw new DomainException(TaskErrors.SubscribeToDeletedTask);
        }

        if (task.CompletedAt is not null)
        {
            throw new DomainException(TaskErrors.SubscribeToCompletedTask);
        }

        if (task.DirectionId is null)
        {
            throw new DomainException(TaskErrors.SubscribeToPrivateTask);
        }

        var subscription = await _subscriptionRepository.GetByTaskAndUserAsync(taskId, currentUserId);

        if (subscription is not null)
        {
            throw new DomainException(TaskErrors.AlreadySubscribedToTask);
        }

        _subscriptionRepository.Add(new TaskSubscription
        {
            TaskId = task.Id,
            UserId = currentUserId
        });

        return await _subscriptionRepository.SaveChangesAsync();
    }

    public async Task<bool> SubscribeToIdeaAsync(Guid ideaId, Guid currentUserId)
    {
        var idea = await _ideaService.GetIdeaOrThrowAsync(ideaId, currentUserId);

        if (idea.DeletedAt is not null)
        {
            throw new DomainException(IdeaErrors.SubscribeToDeletedIdea);
        }

        if (idea.DirectionId is null)
        {
            throw new DomainException(IdeaErrors.SubscribeToPrivateIdea);
        }

        var subscription = await _subscriptionRepository.GetByIdeaAndUserAsync(ideaId, currentUserId);

        if (subscription is not null)
        {
            throw new DomainException(IdeaErrors.AlreadySubscribedToIdea);
        }

        _subscriptionRepository.Add(new IdeaSubscription
        {
            IdeaId = idea.Id,
            UserId = currentUserId
        });

        return await _subscriptionRepository.SaveChangesAsync();
    }

    public async Task<bool> UnsubscribeFromTaskAsync(Guid taskId, Guid currentUserId)
    {
        var taskSubscription = await GetTaskSubscriptionOrThrowAsync(taskId, currentUserId);

        _subscriptionRepository.Delete(taskSubscription);

        return await _subscriptionRepository.SaveChangesAsync();
    }

    public async Task<bool> UnsubscribeFromIdeaAsync(Guid ideaId, Guid currentUserId)
    {
        var ideaSubscription = await GetIdeaSubscriptionOrThrowAsync(ideaId, currentUserId);

        _subscriptionRepository.Delete(ideaSubscription);

        return await _subscriptionRepository.SaveChangesAsync();
    }

    private async Task<TaskSubscription> GetTaskSubscriptionOrThrowAsync(Guid taskId, Guid userId)
    {
        var subscription = await _subscriptionRepository.GetByTaskAndUserAsync(taskId, userId);

        if (subscription == null)
        {
            throw new DomainException(TaskErrors.SubscriptionNotFound);
        }

        return subscription;
    }

    private async Task<IdeaSubscription> GetIdeaSubscriptionOrThrowAsync(Guid ideaId, Guid userId)
    {
        var subscription = await _subscriptionRepository.GetByIdeaAndUserAsync(ideaId, userId);

        if (subscription == null)
        {
            throw new DomainException(IdeaErrors.SubscriptionNotFound);
        }

        return subscription;
    }
}