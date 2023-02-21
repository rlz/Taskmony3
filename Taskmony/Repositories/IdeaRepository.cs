using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models;
using Taskmony.Repositories.Abstract;

namespace Taskmony.Repositories;

public sealed class IdeaRepository : BaseRepository<Idea>, IIdeaRepository
{
    public IdeaRepository(IDbContextFactory<TaskmonyDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<IEnumerable<Idea>> GetIdeasAsync(Guid[]? id, Guid?[] directionId, int? offset,
        int? limit, Guid userId)
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
}