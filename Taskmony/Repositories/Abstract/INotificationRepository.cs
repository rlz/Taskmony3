using Taskmony.Models.Notifications;

namespace Taskmony.Repositories.Abstract;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetNotificationsAsync(Guid[] notifiableIds, DateTime? start, DateTime? end);

    Task<bool> SaveChangesAsync();
}