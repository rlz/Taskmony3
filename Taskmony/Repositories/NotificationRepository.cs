using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models.Notifications;

namespace Taskmony.Repositories;

public sealed class NotificationRepository : INotificationRepository, IDisposable, IAsyncDisposable
{
    private readonly TaskmonyDbContext _context;

    public NotificationRepository(IDbContextFactory<TaskmonyDbContext> contextFactory)
    {
        _context = contextFactory.CreateDbContext();
    }

    public async Task<IEnumerable<Notification>> GetNotificationsAsync(Guid[] notifiableIds, DateTime? start,
        DateTime? end)
    {
        var groupedByNotifiable = _context.Notifications.Where(n => notifiableIds.Contains(n.NotifiableId))
            .GroupBy(n => n.NotifiableId);

        if (start is not null && end is not null)
        {
            return (await groupedByNotifiable
                    .Select(g => g.Where(n => n.ModifiedAt!.Value >= start && n.ModifiedAt.Value <= end))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        if (start is not null)
        {
            return (await groupedByNotifiable
                    .Select(g => g.Where(n => n.ModifiedAt!.Value >= start))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        if (end is not null)
        {
            return (await groupedByNotifiable
                    .Select(g => g.Where(n => n.ModifiedAt!.Value <= end))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        return await _context.Notifications.Where(n => notifiableIds.Contains(n.NotifiableId)).ToListAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}