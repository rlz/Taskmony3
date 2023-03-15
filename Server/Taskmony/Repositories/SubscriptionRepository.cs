using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models.Subscriptions;
using Taskmony.Repositories.Abstract;

namespace Taskmony.Repositories;

public sealed class SubscriptionRepository : BaseRepository<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(IDbContextFactory<TaskmonyDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<IEnumerable<TaskSubscription>> GetByTaskIdsAsync(Guid[] taskIds, int? offset, int? limit)
    {
        var groupedByTask = Context.TaskSubscriptions.Where(s => taskIds.Contains(s.TaskId)).GroupBy(s => s.TaskId);

        if (offset is not null && limit is not null)
        {
            return (await groupedByTask
                    .Select(g => g.OrderBy(s => s.CreatedAt).ThenBy(s => s.Id).Skip(offset.Value).Take(limit.Value))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        if (offset is not null)
        {
            return (await groupedByTask
                    .Select(g => g.OrderBy(s => s.CreatedAt).ThenBy(s => s.Id).Skip(offset.Value))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        if (limit is not null)
        {
            return (await groupedByTask
                    .Select(g => g.OrderBy(s => s.CreatedAt).ThenBy(s => s.Id).Take(limit.Value))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        return await Context.TaskSubscriptions.Where(s => taskIds.Contains(s.TaskId)).ToListAsync();
    }

    public async Task<IEnumerable<IdeaSubscription>> GetByIdeaIdsAsync(Guid[] ideaIds, int? offset, int? limit)
    {
        var groupedByTask = Context.IdeaSubscriptions.Where(s => ideaIds.Contains(s.IdeaId)).GroupBy(s => s.IdeaId);

        if (offset is not null && limit is not null)
        {
            return (await groupedByTask
                    .Select(g => g.OrderBy(s => s.CreatedAt).ThenBy(s => s.Id).Skip(offset.Value).Take(limit.Value))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        if (offset is not null)
        {
            return (await groupedByTask
                    .Select(g => g.OrderBy(s => s.CreatedAt).ThenBy(s => s.Id).Skip(offset.Value))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        if (limit is not null)
        {
            return (await groupedByTask
                    .Select(g => g.OrderBy(s => s.CreatedAt).ThenBy(s => s.Id).Take(limit.Value))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        return await Context.IdeaSubscriptions.Where(s => ideaIds.Contains(s.IdeaId)).ToListAsync();
    }

    public async Task<TaskSubscription?> GetByTaskAndUserAsync(Guid taskId, Guid currentUserId)
    {
        return await Context.TaskSubscriptions.Where(s => s.TaskId == taskId && s.UserId == currentUserId)
            .FirstOrDefaultAsync();
    }

    public async Task<IdeaSubscription?> GetByIdeaAndUserAsync(Guid ideaId, Guid currentUserId)
    {
        return await Context.IdeaSubscriptions.Where(s => s.IdeaId == ideaId && s.UserId == currentUserId)
            .FirstOrDefaultAsync();
    }
}