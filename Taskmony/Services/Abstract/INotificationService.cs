using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;

namespace Taskmony.Services.Abstract;

public interface INotificationService
{
    Task<IEnumerable<Notification>> GetNotificationsByNotifiableIdsAsync(NotifiableType type, Guid[] ids, DateTime? start, DateTime? end, Guid currentUserId);

    Task<bool> NotifyTaskAssignedAsync(Guid taskId, Guid assignerId, Guid assigneeId);

    Task<bool> NotifyTaskUpdatedAsync(Guid taskId, Guid actorId, string field, string? oldValue, string? newValue);
}