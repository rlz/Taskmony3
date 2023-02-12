using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;

namespace Taskmony.Repositories.Abstract;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(NotifiableType type, Guid[] notifiableIds, DateTime? start, DateTime? end, Guid userId);

    Task AddNotificationAsync(Notification notification);

    Task<bool> SaveChangesAsync();
}