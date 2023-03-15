using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;

namespace Taskmony.Repositories.Abstract;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetByUserAsync(NotifiableType type, Guid[] notifiableIds, DateTime? start, DateTime? end, Guid userId);

    Task AddAsync(Notification notification);

    Task<bool> SaveChangesAsync();
}