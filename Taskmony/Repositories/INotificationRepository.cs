using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;

namespace Taskmony.Repositories;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetNotificationsAsync(NotifiableType notifiableType, Guid notifiableId,
        DateTime? start, DateTime? end);

    Task<bool> SaveChangesAsync();
}