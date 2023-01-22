using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;

namespace Taskmony.Repositories;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetNotificationsAsync(Guid notifiableId, DateTime? start, DateTime? end);

    Task<IEnumerable<Notification>> GetNotificationsAsync(Guid[] notifiableIds, DateTime? start, DateTime? end);

    Task<bool> SaveChangesAsync();
}