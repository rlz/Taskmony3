using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Repositories;

public sealed class DirectionRepository : IDirectionRepository, IDisposable, IAsyncDisposable
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

    public async Task<IEnumerable<Direction>> GetDirectionByIdsAsync(Guid[] ids)
    {
        return await _context.Directions
            .Where(d => ids.Contains(d.Id))
            .ToListAsync();
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

    public async Task<ILookup<Guid, Guid>> GetMemberIdsAsync(Guid[] directionIds)
    {
        var memberships = await _context.Memberships
            .Where(m => directionIds.Contains(m.DirectionId))
            .ToListAsync();
        
        return memberships.ToLookup(m => m.DirectionId, m => m.UserId);
    }

    public async Task<bool> AnyMemberWithIdAsync(Guid directionId, Guid memberId)
    {
        return await _context.Memberships.AnyAsync(m => m.DirectionId == directionId && m.UserId == memberId);
    }

    public async Task AddDirectionAsync(Direction direction)
    {
        await _context.Directions.AddAsync(direction);
    }

    public void AddMember(Membership membership)
    {
        _context.Memberships.Add(membership);
    }

    public void RemoveMember(Membership membership)
    {
        _context.Memberships.Remove(membership);
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