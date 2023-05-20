using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models.Comments;
using Taskmony.Models.ValueObjects;
using Taskmony.Repositories.Abstract;

namespace Taskmony.Repositories;

public sealed class CommentRepository : BaseRepository<Comment>, ICommentRepository
{
    public CommentRepository(IDbContextFactory<TaskmonyDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<IEnumerable<Comment>> GetByTaskIdsAsync(IEnumerable<Guid> ids, int? offset, int? limit)
    {
        var groupedByTask = Context.TaskComments.Where(c => ids.Contains(c.TaskId)).GroupBy(c => c.TaskId);

        if (offset is not null && limit is not null)
        {
            return (await groupedByTask
                    .Select(g => g.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id).Skip(offset.Value).Take(limit.Value))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        if (offset is not null)
        {
            return (await groupedByTask
                    .Select(g => g.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id).Skip(offset.Value))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        if (limit is not null)
        {
            return (await groupedByTask
                    .Select(g => g.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id).Take(limit.Value))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        return await Context.TaskComments.Where(c => ids.Contains(c.TaskId)).ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetByIdeaIdsAsync(IEnumerable<Guid> ids, int? offset, int? limit)
    {
        var groupedByIdea = Context.IdeaComments.Where(c => ids.Contains(c.IdeaId)).GroupBy(c => c.IdeaId);

        if (offset is not null && limit is not null)
        {
            return (await groupedByIdea
                    .Select(g => g.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id).Skip(offset.Value).Take(limit.Value))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        if (offset is not null)
        {
            return (await groupedByIdea
                    .Select(g => g.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id).Skip(offset.Value))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        if (limit is not null)
        {
            return (await groupedByIdea
                    .Select(g => g.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id).Take(limit.Value))
                    .ToListAsync())
                .SelectMany(g => g);
        }

        return await Context.IdeaComments.Where(c => ids.Contains(c.IdeaId)).ToListAsync();
    }

    public async Task SoftDeleteTaskCommentsAsync(IEnumerable<Guid> taskIds)
    {
        var toDelete = from c in Context.TaskComments
                       where taskIds.Contains(c.TaskId) && c.DeletedAt == null
                       select c;

        await SoftDeleteCommentsAsync(toDelete);
    }

    public async Task UndeleteTaskCommentsAsync(IEnumerable<Guid> taskIds, DateTime deletedAt)
    {
        var toRecover = from c in Context.TaskComments
                        where taskIds.Contains(c.TaskId) && c.DeletedAt != null && c.DeletedAt.Value >= deletedAt
                        select c;

        await UndeleteCommentsAsync(toRecover);
    }

    public async Task SoftDeleteIdeaCommentsAsync(IEnumerable<Guid> ideaIds)
    {
        var toDelete = from c in Context.IdeaComments
                       where ideaIds.Contains(c.IdeaId) && c.DeletedAt == null
                       select c;

        await SoftDeleteCommentsAsync(toDelete);
    }

    public async Task UndeleteIdeaCommentsAsync(IEnumerable<Guid> ideaIds, DateTime deletedAt)
    {
        var toRecover = from c in Context.IdeaComments
                        where ideaIds.Contains(c.IdeaId) && c.DeletedAt != null && c.DeletedAt.Value >= deletedAt
                        select c;

        await UndeleteCommentsAsync(toRecover);
    }

    private async Task SoftDeleteCommentsAsync(IQueryable<Comment> toDelete)
    {
        await toDelete.ForEachAsync(c => c.UpdateDeletedAt(DeletedAt.From(DateTime.UtcNow)));

        await Context.SaveChangesAsync();
    }

    private async Task UndeleteCommentsAsync(IQueryable<Comment> toRecover)
    {
        await toRecover.ForEachAsync(c => c.UpdateDeletedAt(null));

        await Context.SaveChangesAsync();
    }

    public async Task HardDeleteSoftDeletedComments(DateTime deletedBeforeOrAt)
    {
        await Context.Comments
            .Where(c => c.DeletedAt != null && c.DeletedAt.Value <= deletedBeforeOrAt)
            .ExecuteDeleteAsync();
    }
}