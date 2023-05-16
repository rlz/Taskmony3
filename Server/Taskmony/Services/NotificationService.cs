using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Directions;
using Taskmony.Models.Ideas;
using Taskmony.Models.Notifications;
using Taskmony.Repositories.Abstract;
using Taskmony.Services.Abstract;
using Task = Taskmony.Models.Tasks.Task;

namespace Taskmony.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly ITimeConverter _timeConverter;

    public NotificationService(INotificationRepository notificationRepository, IDirectionRepository directionRepository,
        ITimeConverter timeConverter)
    {
        _notificationRepository = notificationRepository;
        _directionRepository = directionRepository;
        _timeConverter = timeConverter;
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByNotifiableIdsAsync(NotifiableType type, Guid[] ids,
        DateTime? start, DateTime? end, Guid currentUserId)
    {
        if (end < start)
        {
            throw new DomainException(ValidationErrors.EndBeforeStart);
        }

        return await _notificationRepository.GetByUserAsync(type, ids, start, end, currentUserId);
    }

    public async Task<bool> NotifyTaskAssigneeUpdatedAsync(Task task, Guid? oldAssigneeId, Guid modifiedById,
        DateTime? modifiedAt)
    {
        if (task.Assignment is null)
        {
            return await NotifyDirectionEntityUpdatedAsync(task, nameof(Task.Assignment.AssigneeId),
                oldAssigneeId?.ToString(),
                null, modifiedById);
        }

        if (!await ShouldNotifyAsync(task.DirectionId, modifiedById))
        {
            return false;
        }

        return await NotifyTaskAssignedAsync(task, task.Assignment.AssigneeId, modifiedById, modifiedAt);
    }

    private async Task<bool> NotifyTaskAssignedAsync(Task task, Guid assigneeId, Guid modifiedById,
        DateTime? modifiedAt)
    {
        return await CreateNotificationAsync(new Notification(
            actionType: ActionType.TaskAssigned,
            modifiedAt: modifiedAt,
            modifiedById: modifiedById,
            NotifiableType.Task,
            notifiableId: task.Id,
            actionItemType: ActionItemType.User,
            actionItemId: assigneeId));
    }

    public async Task<bool> NotifyDirectionEntityAddedAsync(DirectionEntity entity, DateTime? createdAt,
        Guid createdById)
    {
        if (!await ShouldNotifyAsync(entity.DirectionId, createdById))
        {
            return false;
        }

        if (entity is Task task && task.Assignment is not null)
        {
            await NotifyTaskAssignedAsync(task, task.Assignment.AssigneeId, task.Assignment.AssignedById,
                task.CreatedAt);
        }

        return await NotifyItemAddedAsync(NotifiableType.Direction, entity.DirectionId!.Value,
            entity.ActionItemType, entity.Id, createdById, createdAt);
    }

    public async Task<bool> NotifyDirectionEntityUpdatedAsync(DirectionEntity entity, string field, string? oldValue,
        string? newValue, Guid modifiedById)
    {
        if (!await ShouldNotifyAsync(entity.DirectionId, modifiedById))
        {
            return false;
        }

        return entity switch
        {
            Task task => await NotifyItemUpdatedAsync(NotifiableType.Task, task.Id, modifiedById, field, oldValue,
                newValue),
            Idea idea => await NotifyItemUpdatedAsync(NotifiableType.Idea, idea.Id, modifiedById, field, oldValue,
                newValue),
            _ => false
        };
    }

    public async Task<bool> NotifyDirectionEntityDeletedAtUpdatedAsync(DirectionEntity entity, DateTime? oldDeletedAt,
        DateTime? newDeletedAt, Guid modifiedById)
    {
        if (!await ShouldNotifyAsync(entity.DirectionId, modifiedById))
        {
            return false;
        }

        if (newDeletedAt is not null)
        {
            return await NotifyItemRemovedAsync(NotifiableType.Direction, entity.DirectionId!.Value,
                entity.ActionItemType, entity.Id, modifiedById, newDeletedAt);
        }

        var newValue = newDeletedAt is null ? null : _timeConverter.DateTimeToString(newDeletedAt.Value);
        var oldValue = oldDeletedAt is null ? null : _timeConverter.DateTimeToString(oldDeletedAt.Value);

        return await NotifyItemUpdatedAsync(NotifiableType.Task, entity.Id, modifiedById,
            nameof(Task.DeletedAt), oldValue, newValue);
    }

    public async Task<bool> NotifyDirectionEntityMovedAsync(DirectionEntity entity, Guid? oldDirectionId,
        Guid modifiedById, DateTime? modifiedAt)
    {
        if (!await ShouldNotifyAsync(entity.DirectionId, modifiedById) &&
            !await ShouldNotifyAsync(oldDirectionId, modifiedById))
        {
            return false;
        }

        if (entity.DirectionId is not null)
        {
            await NotifyItemAddedAsync(NotifiableType.Direction, entity.DirectionId!.Value,
                entity.ActionItemType, entity.Id, modifiedById, modifiedAt);
        }

        if (oldDirectionId is not null)
        {
            await NotifyItemRemovedAsync(NotifiableType.Direction, oldDirectionId.Value,
                entity.ActionItemType, entity.Id, modifiedById, modifiedAt);
        }

        return true;
    }

    public async Task<bool> NotifyCommentAddedAsync(DirectionEntity entity, Guid commentId, Guid createdById,
        DateTime? createdAt)
    {
        if (!await ShouldNotifyAsync(entity.DirectionId, createdById))
        {
            return false;
        }

        return entity switch
        {
            Task task => await NotifyItemAddedAsync(NotifiableType.Task, task.Id, ActionItemType.Comment, commentId,
                createdById, createdAt),
            Idea idea => await NotifyItemAddedAsync(NotifiableType.Idea, idea.Id, ActionItemType.Comment, commentId,
                createdById, createdAt),
            _ => false
        };
    }

    public async Task<bool> NotifyDirectionUpdatedAsync(Guid directionId, string field, string? oldValue,
        string? newValue, Guid modifiedById)
    {
        if (!await ShouldNotifyAsync(directionId, modifiedById))
        {
            return false;
        }

        return await NotifyItemUpdatedAsync(NotifiableType.Direction, directionId, modifiedById, field, oldValue,
            newValue);
    }

    public async Task<bool> NotifyDirectionMemberAddedAsync(Guid directionId, Guid memberId, DateTime? modifiedAt,
        Guid modifiedById)
    {
        if (!await ShouldNotifyAsync(directionId, modifiedById))
        {
            return false;
        }

        return await NotifyItemAddedAsync(NotifiableType.Direction, directionId, ActionItemType.User, memberId,
            modifiedById, modifiedAt);
    }

    public async Task<bool> NotifyDirectionMemberRemovedAsync(Guid directionId, Guid memberId, DateTime? modifiedAt,
        Guid modifiedById)
    {
        if (!await ShouldNotifyAsync(directionId, modifiedById))
        {
            return false;
        }

        return await NotifyItemRemovedAsync(NotifiableType.Direction, directionId, ActionItemType.User, memberId,
            modifiedById, modifiedAt);
    }

    /// <summary>
    /// Checks if direction id is not null and if there are any members other than owner/modifier in direction
    /// </summary>
    /// <param name="directionId">Id of entity direction</param>
    /// <param name="modifiedById">User who updated entity</param>
    /// <returns>Whether notification should be created</returns>
    private async Task<bool> ShouldNotifyAsync(Guid? directionId, Guid modifiedById)
    {
        return directionId is not null &&
               await _directionRepository.AnyMemberOtherThanUserInDirectionAsync(directionId.Value,
                   modifiedById);
    }

    private async Task<bool> NotifyItemUpdatedAsync(NotifiableType type, Guid notifiableId, Guid modifiedById,
        string field, string? oldValue, string? newValue)
    {
        return await CreateNotificationAsync(new Notification(
            actionType: ActionType.ItemUpdated,
            modifiedAt: DateTime.UtcNow,
            notifiableId: notifiableId,
            notifiableType: type,
            modifiedById: modifiedById,
            field: field,
            oldValue: oldValue,
            newValue: newValue));
    }

    private async Task<bool> NotifyItemAddedAsync(NotifiableType notifiableType, Guid notifiableId,
        ActionItemType itemType, Guid itemId, Guid modifiedById, DateTime? modifiedAtUtc)
    {
        return await CreateNotificationAsync(new Notification(
            actionType: ActionType.ItemAdded,
            notifiableId: notifiableId,
            notifiableType: notifiableType,
            modifiedAt: modifiedAtUtc,
            modifiedById: modifiedById,
            actionItemId: itemId,
            actionItemType: itemType));
    }

    private async Task<bool> NotifyItemRemovedAsync(NotifiableType notifiableType, Guid notifiableId,
        ActionItemType itemType, Guid itemId, Guid modifiedById, DateTime? modifiedAtUtc)
    {
        return await CreateNotificationAsync(new Notification(
            actionType: ActionType.ItemDeleted,
            notifiableId: notifiableId,
            notifiableType: notifiableType,
            modifiedAt: modifiedAtUtc,
            modifiedById: modifiedById,
            actionItemId: itemId,
            actionItemType: itemType));
    }

    private async Task<bool> CreateNotificationAsync(Notification notification)
    {
        await _notificationRepository.AddAsync(notification);

        return await _notificationRepository.SaveChangesAsync();
    }
}