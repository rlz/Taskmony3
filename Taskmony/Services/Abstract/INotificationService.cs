using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;

namespace Taskmony.Services.Abstract;

public interface INotificationService
{
    Task<IEnumerable<Notification>> GetNotificationsByNotifiableIdsAsync(NotifiableType type, Guid[] ids, 
        DateTime? start, DateTime? end, Guid currentUserId);

    Task<bool> NotifyTaskAssignedAsync(Guid taskId, Guid assignerId, Guid assigneeId);

    Task<bool> NotifyItemUpdatedAsync(NotifiableType type, Guid notifiableId, Guid actorId, string field, 
        string? oldValue, string? newValue);

    Task<bool> NotifyItemAddedAsync(NotifiableType notifiableType, Guid notifiableId, ActionItemType itemType, 
        Guid itemId, Guid modifiedById, DateTime? modifiedAtUtc);

    Task<bool> NotifyItemRemovedAsync(NotifiableType notifiableType, Guid notifiableId, ActionItemType itemType,
        Guid itemId, Guid modifiedById, DateTime? modifiedAtUtc);
}