using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Repositories;

public sealed class IdeaRepository : IIdeaRepository, IDisposable, IAsyncDisposable
{
    private readonly TaskmonyDbContext _context;

    public IdeaRepository(IDbContextFactory<TaskmonyDbContext> contextFactory)
    {
        _context = contextFactory.CreateDbContext();
    }

    public async Task<IEnumerable<Idea>> GetIdeasAsync(Guid[]? id, Guid?[] directionId, int? offset,
        int? limit, Guid userId)
    {
        var query = _context.Ideas.AsQueryable();

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

    public async Task<Idea?> GetIdeaByIdAsync(Guid id)
    {
        return await _context.Ideas.FindAsync(id);
    }

    public async Task<IEnumerable<Idea>> GetIdeaByIdsAsync(Guid[] ids)
    {
        return await _context.Ideas.Where(i => ids.Contains(i.Id)).ToListAsync();
    }

    public async Task AddIdeaAsync(Idea idea)
    {
        await _context.Ideas.AddAsync(idea);
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