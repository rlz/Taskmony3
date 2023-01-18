using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models;

namespace Taskmony.Repositories;

public class DirectionRepository : IDirectionRepository, IAsyncDisposable
{
    private readonly TaskmonyDbContext _context;

    public DirectionRepository(IDbContextFactory<TaskmonyDbContext> contextFactory)
    {
        _context = contextFactory.CreateDbContext();
    }

    public async Task<Direction?> GetDirectionByIdAsync(Guid id)
    {
        return await _context.Directions.FindAsync(id);
    }

    public async Task<IEnumerable<Direction>> GetDirectionsAsync(Guid[]? id, int? offset, int? limit, Guid userId)
    {
        var query = _context.Directions.AsQueryable();

        query = id is null
            ? query.Where(d => d.CreatedById == userId || d.Members!.Any(m => m.Id == userId))
            : query.Where(d => id.Contains(d.Id) && (d.CreatedById == userId || d.Members!.Any(m => m.Id == userId)));

        query = AddPagination(query, offset, limit);

        return await query.ToListAsync();
    }

    private IQueryable<Direction> AddPagination(IQueryable<Direction> query, int? offset, int? limit)
    {
        if (offset is not null)
        {
            query = query
                .OrderBy(d => d.CreatedAt)
                .ThenBy(d => d.Id)
                .Skip(offset.Value);
        }

        if (limit is not null)
        {
            query = query
                .OrderBy(d => d.CreatedAt)
                .ThenBy(d => d.Id)
                .Take(limit.Value);
        }

        return query;
    }

    public async Task<IEnumerable<Guid>> GetMemberIdsAsync(Guid directionId)
    {
        return await _context.Memberships
            .Where(m => m.DirectionId == directionId)
            .Select(m => m.UserId)
            .ToListAsync();
    }

    public async Task<bool> AnyMemberWithId(Guid directionId, Guid memberId)
    {
        return await _context.Memberships.AnyAsync(m => m.DirectionId == directionId && m.UserId == memberId);
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