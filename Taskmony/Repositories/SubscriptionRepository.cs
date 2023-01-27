using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models.Subscriptions;

namespace Taskmony.Repositories;

public sealed class SubscriptionRepository : ISubscriptionRepository, IDisposable, IAsyncDisposable
{
    private readonly TaskmonyDbContext _context;

    public SubscriptionRepository(IDbContextFactory<TaskmonyDbContext> contextFactory)
    {
        _context = contextFactory.CreateDbContext();
    }

    public async Task<IEnumerable<TaskSubscription>> GetTaskSubscriptionsAsync(Guid taskId)
    {
        return await _context.TaskSubscriptions.Where(s => s.TaskId == taskId).ToListAsync();
    }

    public async Task<IEnumerable<TaskSubscription>> GetTaskSubscriptionsAsync(Guid[] taskIds, int? offset, int? limit)
    {
        var groupedByTask = _context.TaskSubscriptions.Where(s => taskIds.Contains(s.TaskId)).GroupBy(s => s.TaskId);

        if (offset is not null && limit is not null)
        {
            return (await groupedByTask
                    .Select(g => g.OrderBy(s => s.SubscribedAt).ThenBy(s => s.Id).Skip(offset.Value).Take(limit.Value))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        if (offset is not null)
        {
            return (await groupedByTask
                    .Select(g => g.OrderBy(s => s.SubscribedAt).ThenBy(s => s.Id).Skip(offset.Value))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        if (limit is not null)
        {
            return (await groupedByTask
                    .Select(g => g.OrderBy(s => s.SubscribedAt).ThenBy(s => s.Id).Take(limit.Value))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        return await _context.TaskSubscriptions.Where(s => taskIds.Contains(s.TaskId)).ToListAsync();
    }

    public async Task<IEnumerable<IdeaSubscription>> GetIdeaSubscriptionsAsync(Guid ideaId)
    {
        return await _context.IdeaSubscriptions.Where(s => s.IdeaId == ideaId).ToListAsync();
    }

    public async Task<IEnumerable<IdeaSubscription>> GetIdeaSubscriptionsAsync(Guid[] ideaIds, int? offset, int? limit)
    {
        var groupedByTask = _context.IdeaSubscriptions.Where(s => ideaIds.Contains(s.IdeaId)).GroupBy(s => s.IdeaId);

        if (offset is not null && limit is not null)
        {
            return (await groupedByTask
                    .Select(g => g.OrderBy(s => s.SubscribedAt).ThenBy(s => s.Id).Skip(offset.Value).Take(limit.Value))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        if (offset is not null)
        {
            return (await groupedByTask
                    .Select(g => g.OrderBy(s => s.SubscribedAt).ThenBy(s => s.Id).Skip(offset.Value))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        if (limit is not null)
        {
            return (await groupedByTask
                    .Select(g => g.OrderBy(s => s.SubscribedAt).ThenBy(s => s.Id).Take(limit.Value))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        return await _context.IdeaSubscriptions.Where(s => ideaIds.Contains(s.IdeaId)).ToListAsync();
    }

    public void AddTaskSubscription(TaskSubscription subscription)
    {
        _context.TaskSubscriptions.Add(subscription);
    }

    public void AddIdeaSubscription(IdeaSubscription subscription)
    {
        _context.IdeaSubscriptions.Add(subscription);
    }

    public void RemoveTaskSubscription(TaskSubscription subscription)
    {
        _context.TaskSubscriptions.Remove(subscription);
    }

    public void RemoveIdeaSubscription(IdeaSubscription subscription)
    {
        _context.IdeaSubscriptions.Remove(subscription);
    }

    public async Task<TaskSubscription?> GetTaskSubscriptionAsync(Guid taskId, Guid currentUserId)
    {
        return await _context.TaskSubscriptions.Where(s => s.TaskId == taskId && s.UserId == currentUserId)
            .FirstOrDefaultAsync();
    }

    public async Task<IdeaSubscription?> GetIdeaSubscriptionAsync(Guid ideaId, Guid currentUserId)
    {
        return await _context.IdeaSubscriptions.Where(s => s.IdeaId == ideaId && s.UserId == currentUserId)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _context.DisposeAsync();
    }
}