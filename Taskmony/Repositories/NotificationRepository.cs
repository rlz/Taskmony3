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
            NotifiableType.Task =>
                from n in _context.Notifications
                join t in _context.Tasks on n.NotifiableId equals t.Id
                from s in _context.TaskSubscriptions.Where(s => s.TaskId == t.Id && n.ModifiedAt >= s.CreatedAt)
                    .DefaultIfEmpty() // left join on two columns
                where notifiableIds.Contains(n.NotifiableId) && n.ModifiedById != userId &&
                      (s.UserId == userId || n.ActionType == ActionType.TaskAssigned && t.AssigneeId == userId)
                select n,
            NotifiableType.Idea =>
                from n in _context.Notifications
                join i in _context.Ideas on n.NotifiableId equals i.Id
                from s in _context.IdeaSubscriptions.Where(s => s.IdeaId == i.Id && n.ModifiedAt >= s.CreatedAt)
                    .DefaultIfEmpty()
                where notifiableIds.Contains(n.NotifiableId) && n.ModifiedById != userId && (s.UserId == userId)
                select n,
            NotifiableType.Direction =>
                from n in _context.Notifications
                from m in _context.Memberships.Where(m => n.NotifiableId == m.DirectionId && n.ModifiedAt >= m.CreatedAt)
                where notifiableIds.Contains(n.NotifiableId) && n.ModifiedById != userId && m.UserId == userId ||
                      (n.ActionItemType == ActionItemType.User && n.ActionItemId == userId) // member added/removed/left
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