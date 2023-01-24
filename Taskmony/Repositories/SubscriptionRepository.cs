using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models.Subscriptions;

namespace Taskmony.Repositories;

public class SubscriptionRepository : ISubscriptionRepository, IAsyncDisposable
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

    public async Task<IEnumerable<TaskSubscription>> GetTaskSubscriptionsAsync(Guid[] taskIds)
    {
        return await _context.TaskSubscriptions.Where(s => taskIds.Contains(s.TaskId)).ToListAsync();
    }

    public async Task<IEnumerable<IdeaSubscription>> GetIdeaSubscriptionsAsync(Guid ideaId)
    {
        return await _context.IdeaSubscriptions.Where(s => s.IdeaId == ideaId).ToListAsync();
    }

    public async Task<IEnumerable<IdeaSubscription>> GetIdeaSubscriptionsAsync(Guid[] ideaIds)
    {
        return await _context.IdeaSubscriptions.Where(s => ideaIds.Contains(s.IdeaId)).ToListAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public ValueTask DisposeAsync()
    {
        return _context.DisposeAsync();
    }
}