using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Repositories;

public abstract class BaseRepository<TEntity> : IDisposable, IAsyncDisposable where TEntity : Entity
{
    protected readonly TaskmonyDbContext Context;

    protected BaseRepository(IDbContextFactory<TaskmonyDbContext> contextFactory)
    {
        Context = contextFactory.CreateDbContext();
    }

    public async Task AddAsync(TEntity entity)
    {
        await Context.Set<TEntity>().AddAsync(entity);
    }
    
    public void Add(TEntity entity)
    {
        Context.Set<TEntity>().Add(entity);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await Context.Set<TEntity>().AddRangeAsync(entities);
    }
    
    public void AddRange(IEnumerable<TEntity> entities)
    {
        Context.Set<TEntity>().AddRange(entities);
    }

    public void Delete(TEntity entity)
    {
        Context.Set<TEntity>().Remove(entity);
    }

    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        Context.Set<TEntity>().RemoveRange(entities);
    }

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await Context.Set<TEntity>().FindAsync(id);
    }

    public async Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        return await Context.Set<TEntity>().Where(e => ids.Contains(e.Id)).ToListAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await Context.SaveChangesAsync() > 0;
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return Context.DisposeAsync();
    }
}