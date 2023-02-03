using Microsoft.EntityFrameworkCore;
using Taskmony.Data;

namespace Taskmony.Repositories;

public sealed class TaskRepository : ITaskRepository, IDisposable, IAsyncDisposable
{
    private readonly TaskmonyDbContext _context;

    public TaskRepository(IDbContextFactory<TaskmonyDbContext> contextFactory)
    {
        _context = contextFactory.CreateDbContext();
    }

    public async Task<IEnumerable<Models.Task>> GetTasksAsync(Guid[]? id, Guid?[] directionId, int? offset,
        int? limit, Guid userId)
    {
        var query = _context.Tasks.AsQueryable();

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

    private IQueryable<Models.Task> AddPagination(IQueryable<Models.Task> query, int? offset, int? limit)
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

    public async Task<IEnumerable<Models.Task>> GetTasksByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _context.Tasks.Where(t => ids.Contains(t.Id)).ToListAsync();
    }

    public async Task AddTaskAsync(Models.Task task)
    {
        await _context.Tasks.AddAsync(task);
    }

    public async Task AddTasksAsync(IEnumerable<Models.Task> tasks)
    {
        await _context.Tasks.AddRangeAsync(tasks);
    }

    public async Task<Models.Task?> GetTaskByIdAsync(Guid id)
    {
        return await _context.Tasks.FindAsync(id);
    }

    public async Task<IEnumerable<Models.Task>> GetNotCompletedTasksAsync(Guid id)
    {
        return await _context.Tasks.Where(t => t.GroupId == id && t.CompletedAt == null).ToListAsync();
    }

    public async Task<IEnumerable<Models.Task>> GetActiveTasksAsync(Guid groupId)
    {
        return await _context.Tasks
            .Where(t => t.GroupId == groupId && t.CompletedAt == null && t.DeletedAt == null)
            .ToListAsync();
    }

    public void DeleteTasks(IEnumerable<Models.Task> tasks)
    {
        _context.Tasks.RemoveRange(tasks);
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