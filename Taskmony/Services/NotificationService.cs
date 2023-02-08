using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Notifications;
using Taskmony.Repositories.Abstract;
using Taskmony.Services.Abstract;

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
        if (end < start)
        {
            throw new DomainException(ValidationErrors.EndBeforeStart);
        }

        return await _notificationRepository.GetNotificationsAsync(ids, start, end);
    }
}