using Taskmony.Models.Notifications;

namespace Taskmony.Services.Abstract;

public interface INotificationService
{
    Task<IEnumerable<Notification>> GetNotificationsByNotifiableIdsAsync(Guid[] ids, DateTime? start, DateTime? end);
}