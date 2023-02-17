using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;
using Taskmony.Repositories.Abstract;
using Taskmony.Services.Abstract;

namespace Taskmony.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByNotifiableIdsAsync(NotifiableType type, Guid[] ids,
        DateTime? start, DateTime? end, Guid currentUserId)
    {
        if (end < start)
        {
            throw new DomainException(ValidationErrors.EndBeforeStart);
        }

        return await _notificationRepository.GetUserNotificationsAsync(type, ids, start, end, currentUserId);
    }

    public async Task<bool> NotifyTaskAssignedAsync(Guid taskId, Guid assignerId, Guid assigneeId)
    {
        return await CreateNotificationAsync(new Notification
        {
            NotifiableId = taskId,
            NotifiableType = NotifiableType.Task,
            ActionItemId = assigneeId,
            ActionItemType = ActionItemType.User,
            ActionType = ActionType.TaskAssigned,
            ModifiedById = assignerId
        });
    }

    public async Task<bool> NotifyItemUpdatedAsync(NotifiableType type, Guid notifiableId, Guid modifiedById,
        string field, string? oldValue, string? newValue)
    {
        return await CreateNotificationAsync(new Notification
        {
            NotifiableId = notifiableId,
            NotifiableType = type,
            ModifiedById = modifiedById,
            ActionType = ActionType.ItemUpdated,
            Field = field,
            OldValue = oldValue,
            NewValue = newValue
        });
    }

    public async Task<bool> NotifyItemAddedAsync(NotifiableType notifiableType, Guid notifiableId,
        ActionItemType itemType, Guid itemId, Guid modifiedById, DateTime? modifiedAtUtc)
    {
        return await CreateNotificationAsync(new Notification
        {
            NotifiableId = notifiableId,
            NotifiableType = notifiableType,
            ActionType = ActionType.ItemAdded,
            ActionItemId = itemId,
            ActionItemType = itemType,
            ModifiedAt = modifiedAtUtc,
            ModifiedById = modifiedById
        });
    }

    public async Task<bool> NotifyItemRemovedAsync(NotifiableType notifiableType, Guid notifiableId,
        ActionItemType itemType, Guid itemId, Guid modifiedById, DateTime? modifiedAtUtc)
    {
        return await CreateNotificationAsync(new Notification
        {
            NotifiableId = notifiableId,
            NotifiableType = notifiableType,
            ActionType = ActionType.ItemDeleted,
            ActionItemId = itemId,
            ActionItemType = itemType,
            ModifiedAt = modifiedAtUtc,
            ModifiedById = modifiedById
        });
    }

    private async Task<bool> CreateNotificationAsync(Notification notification)
    {
        notification.ModifiedAt ??= DateTime.UtcNow;

        await _notificationRepository.AddNotificationAsync(notification);

        return await _notificationRepository.SaveChangesAsync();
    }
}