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
        await _notificationRepository.AddNotificationAsync(new Notification
        {
            NotifiableId = taskId,
            NotifiableType = NotifiableType.Task,
            ActionItemId = assigneeId,
            ActionType = ActionType.TaskAssigned,
            ModifiedAt = DateTime.UtcNow,
            ActorId = assignerId
        });

        return await _notificationRepository.SaveChangesAsync();
    }

    public async Task<bool> NotifyTaskUpdatedAsync(Guid taskId, Guid actorId, string field, string? oldValue, string? newValue)
    {
        await _notificationRepository.AddNotificationAsync(new Notification
        {
            NotifiableId = taskId,
            NotifiableType = NotifiableType.Task,
            ActionItemId = taskId,
            ActionType = ActionType.ItemUpdated,
            ModifiedAt = DateTime.UtcNow,
            ActorId = actorId,
            Field = field,
            OldValue = oldValue,
            NewValue = newValue
        });

        return await _notificationRepository.SaveChangesAsync();
    }
}