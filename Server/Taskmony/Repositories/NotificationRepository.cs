using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;
using Taskmony.Repositories.Abstract;

namespace Taskmony.Repositories;

public sealed class NotificationRepository : BaseRepository<Notification>, INotificationRepository
{
    public NotificationRepository(IDbContextFactory<TaskmonyDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<IEnumerable<Notification>> GetByUserAsync(NotifiableType type, Guid[] notifiableIds,
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
        // user gets notifications if
        return type switch
        {
            // user is a creator or subscriber or assignee or assignor
            NotifiableType.Task =>
                from n in Context.Notifications
                join t in Context.Tasks on n.NotifiableId equals t.Id
                from s in Context.TaskSubscriptions.Where(s => s.TaskId == t.Id && n.ModifiedAt >= s.CreatedAt)
                    .DefaultIfEmpty() // left join on two columns
                from a in Context.Assignments.Where(a => a.TaskId == t.Id).DefaultIfEmpty()
                where notifiableIds.Contains(n.NotifiableId) && n.ModifiedById != userId &&
                      (t.CreatedById == userId || s.UserId == userId ||
                       n.ActionItemType == ActionItemType.User && n.ActionItemId == userId ||
                       a.AssigneeId == userId && a.CreatedAt <= n.ModifiedAt ||
                       a.AssignedById == userId && a.CreatedAt <= n.ModifiedAt)
                select n,
            // user is a creator or subscriber
            NotifiableType.Idea =>
                from n in Context.Notifications
                join i in Context.Ideas on n.NotifiableId equals i.Id
                from s in Context.IdeaSubscriptions.Where(s => s.IdeaId == i.Id && n.ModifiedAt >= s.CreatedAt)
                    .DefaultIfEmpty()
                where notifiableIds.Contains(n.NotifiableId) && n.ModifiedById != userId &&
                      (i.CreatedById == userId || s.UserId == userId ||
                       n.ActionItemType == ActionItemType.User && n.ActionItemId == userId)
                select n,
            // user is a member
            NotifiableType.Direction =>
                from n in Context.Notifications
                from m in Context.Memberships.Where(
                    m => n.NotifiableId == m.DirectionId && n.ModifiedAt >= m.CreatedAt)
                where notifiableIds.Contains(n.NotifiableId) && n.ModifiedById != userId &&
                      (m.UserId == userId || n.ActionItemType == ActionItemType.User && n.ActionItemId == userId)
                select n,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}