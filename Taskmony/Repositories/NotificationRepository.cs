using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;

namespace Taskmony.Repositories;

public class NotificationRepository : INotificationRepository, IAsyncDisposable
{
    private readonly TaskmonyDbContext _context;

    public NotificationRepository(IDbContextFactory<TaskmonyDbContext> contextFactory)
    {
        _context = contextFactory.CreateDbContext();
    }

    public async Task<IEnumerable<Notification>> GetNotificationsAsync(NotifiableType notifiableType,
        Guid notifiableId, DateTime? start, DateTime? end)
    {
        var query = _context.Notifications.AsQueryable();

        query = query.Where(n => n.NotifiableType == notifiableType && n.NotifiableId == notifiableId);

        if (start is not null)
        {
            query = query.Where(n => n.ModifiedAt >= start.Value);
        }

        if (end is not null)
        {
            query = query.Where(n => n.ModifiedAt <= end.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}