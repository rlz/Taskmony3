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

    public async Task<IEnumerable<Direction>> GetUserDirectionsAsync(Guid userId)
    {
        return await _context.Directions
            .Where(d => d.CreatedById == userId || d.Members!.Any(m => m.Id == userId))
            .ToListAsync();
    }
    
    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }

    public ValueTask DisposeAsync()
    {
        return _context.DisposeAsync();
    }
}