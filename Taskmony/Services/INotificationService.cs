using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;

namespace Taskmony.Services;

public interface INotificationService
{
    Task<IActionItem?> GetActionItemAsync(ActionItemType actionItemType, Guid actionItemId, Guid currentUserId);

    Task<IEnumerable<Notification>> GetNotificationsByNotifiableIdAsync(Guid id, DateTime? start, DateTime? end);

    Task<IEnumerable<Notification>> GetNotificationsByNotifiableIdsAsync(Guid[] ids, DateTime? start, DateTime? end);
}