using Microsoft.EntityFrameworkCore;
using Taskmony.Data;

namespace Taskmony.Repositories;

public class IdeaRepository : IIdeaRepository, IAsyncDisposable
{
    private readonly TaskmonyDbContext _context;

    public IdeaRepository(IDbContextFactory<TaskmonyDbContext> contextFactory)
    {
        _context = contextFactory.CreateDbContext();
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