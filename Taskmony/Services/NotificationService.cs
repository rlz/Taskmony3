using Taskmony.Models.Notifications;
using Taskmony.Repositories;

namespace Taskmony.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByNotifiableIdsAsync(Guid[] ids, DateTime? start,
        DateTime? end)
    {
        return await _notificationRepository.GetNotificationsAsync(ids, start, end);
    }
}