using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;

namespace Taskmony.Services;

public interface INotificationService
{
    Task<IActionItem?> GetActionItemAsync(ActionItemType actionItemType, Guid actionItemId, Guid currentUserId);

    Task<IEnumerable<Notification>> GetTaskNotificationsAsync(Guid taskId, DateTime? start, DateTime? end);

    Task<IEnumerable<Notification>> GetIdeaNotificationsAsync(Guid ideaId, DateTime? start, DateTime? end);

    Task<IEnumerable<Notification>> GetDirectionNotificationsAsync(Guid directionId, DateTime? start, DateTime? end);
}