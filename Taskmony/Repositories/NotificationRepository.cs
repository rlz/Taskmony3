using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;
using Taskmony.Repositories.Abstract;

namespace Taskmony.Repositories;

public sealed class NotificationRepository : INotificationRepository, IDisposable, IAsyncDisposable
{
    private readonly TaskmonyDbContext _context;

    public NotificationRepository(IDbContextFactory<TaskmonyDbContext> contextFactory)
    {
        _context = contextFactory.CreateDbContext();
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(NotifiableType type, Guid[] notifiableIds,
        DateTime? start, DateTime? end, Guid userId)
    {
        var query = GetUserNotifications(type, notifiableIds, userId);
        var groupedByNotifiable = query.GroupBy(n => n.NotifiableId);

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

        return await query.ToListAsync();
    }

    private IQueryable<Notification> GetUserNotifications(NotifiableType type, Guid[] notifiableIds, Guid userId)
    {
        return type switch
        {
            NotifiableType.Task => from n in _context.Notifications
                                   join t in _context.Tasks on n.NotifiableId equals t.Id
                                   join s in _context.TaskSubscriptions on t.Id equals s.TaskId into grouping
                                   from s in grouping.DefaultIfEmpty() // that's a great way to perform left join
                                   where s.UserId == userId || t.CreatedById == userId || t.AssigneeId == userId
                                   where notifiableIds.Contains(n.NotifiableId)
                                   select n,
            NotifiableType.Idea => from n in _context.Notifications
                                   join i in _context.Ideas on n.NotifiableId equals i.Id
                                   join s in _context.IdeaSubscriptions on i.Id equals s.IdeaId into grouping
                                   from s in grouping.DefaultIfEmpty()
                                   where s.UserId == userId || i.CreatedById == userId
                                   where notifiableIds.Contains(n.NotifiableId)
                                   select n,
            NotifiableType.Direction => from n in _context.Notifications
                                        join m in _context.Memberships on n.NotifiableId equals m.DirectionId
                                        where m.UserId == userId
                                        where notifiableIds.Contains(n.NotifiableId)
                                        select n,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }

    public async Task AddNotificationAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
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