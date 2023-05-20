using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models.Ideas;
using Taskmony.Models.ValueObjects;
using Taskmony.Repositories.Abstract;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Repositories;

public sealed class IdeaRepository : BaseRepository<Idea>, IIdeaRepository
{
    public IdeaRepository(IDbContextFactory<TaskmonyDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<IEnumerable<Idea>> GetAsync(Guid[]? id, Guid?[] directionId, bool? deleted,
        DateTime? lastDeletedAt, int? offset, int? limit, Guid userId)
    {
        var query = Context.Ideas.AsQueryable();

        if (directionId.Contains(null))
        {
            query = query.Where(t => t.DirectionId != null && directionId.Contains(t.DirectionId)
                                     || t.DirectionId == null && t.CreatedById == userId);
        }
        else
        {
            query = query.Where(t => t.DirectionId != null && directionId.Contains(t.DirectionId));
        }

        if (id is not null)
        {
            query = query.Where(t => id.Contains(t.Id));
        }

        if (deleted != null)
        {
            query = deleted.Value
                ? query.Where(t => t.DeletedAt != null && (lastDeletedAt == null || t.DeletedAt.Value <= lastDeletedAt))
                : query.Where(t => t.DeletedAt == null);
        }

        query = AddPagination(query, offset, limit);

        return await query.ToListAsync();
    }

    private IQueryable<Idea> AddPagination(IQueryable<Idea> query, int? offset, int? limit)
    {
        if (offset is not null)
        {
            query = query
                .OrderBy(t => t.CreatedAt)
                .ThenBy(t => t.Id)
                .Skip(offset.Value);
        }

        if (limit is not null)
        {
            query = query
                .OrderBy(t => t.CreatedAt)
                .ThenBy(t => t.Id)
                .Take(limit.Value);
        }

        return query;
    }

    public async Task SoftDeleteDirectionIdeasAndCommentsAsync(Guid directionId)
    {
        var now = DateTime.UtcNow;

        var ideas = from i in Context.Ideas
                    where i.DirectionId == directionId && i.DeletedAt == null
                    select i;

        var comments = from i in Context.Ideas
                       where i.DirectionId == directionId && i.DeletedAt == null
                       from c in Context.IdeaComments.Where(c => c.IdeaId == i.Id && c.DeletedAt == null)
                       where i.DirectionId == directionId && i.DeletedAt == null
                       select c;

        await ideas.ForEachAsync(i => i.UpdateDeletedAt(DeletedAt.From(now)));
        await comments.ForEachAsync(c => c.UpdateDeletedAt(DeletedAt.From(now)));

        await Context.SaveChangesAsync();
    }

    public async Task UndeleteDirectionIdeasAndComments(Guid directionId, DateTime deletedAt)
    {
        var ideas = from i in Context.Ideas
                    where i.DirectionId == directionId && i.DeletedAt != null && i.DeletedAt.Value >= deletedAt
                    select i;

        var comments = from i in Context.Ideas
                       where i.DirectionId == directionId && i.DeletedAt != null && i.DeletedAt.Value >= deletedAt
                       from c in Context.IdeaComments.Where(c =>
                           c.IdeaId == i.Id && c.DeletedAt != null && c.DeletedAt.Value >= deletedAt)
                       where i.DirectionId == directionId && i.DeletedAt != null && i.DeletedAt.Value >= deletedAt
                       select c;

        await ideas.ForEachAsync(i => i.UpdateDeletedAt(null));
        await comments.ForEachAsync(c => c.UpdateDeletedAt(null));

        await Context.SaveChangesAsync();
    }

    public async Task HardDeleteSoftDeletedIdeasWithChildren(DateTime deletedBeforeOrAt)
    {
        // Comments are deleted with cascade
        await Context.Ideas
            .Where(i => i.DeletedAt != null && i.DeletedAt.Value <= deletedBeforeOrAt)
            .ExecuteDeleteAsync();
    }
}