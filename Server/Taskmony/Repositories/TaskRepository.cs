using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Repositories.Abstract;

namespace Taskmony.Repositories;

public sealed class TaskRepository : BaseRepository<Models.Task>, ITaskRepository
{
    public TaskRepository(IDbContextFactory<TaskmonyDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<IEnumerable<Models.Task>> GetAsync(Guid[]? id, Guid?[] directionId, int? offset,
        int? limit, Guid userId)
    {
        var query = Context.Tasks.AsQueryable();

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

    public new async Task<Models.Task?> GetByIdAsync(Guid id)
    {
        return await Context.Tasks
            .Include(t => t.Assignment)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Models.Task>> GetActiveTasksAsync(Guid groupId)
    {
        return await Context.Tasks
            .Where(t => t.GroupId == groupId && t.CompletedAt == null && t.DeletedAt == null)
            .Include(t => t.Assignment)
            .ToListAsync();
    }

    public async Task<IEnumerable<Models.Task>> GetTasksByGroupIdAsync(Guid groupId)
    {
        return await Context.Tasks.Where(t => t.GroupId == groupId).ToListAsync();
    }

    public async Task<IEnumerable<Models.Task>> GetByDirectionIdAndAssigneeIdAsync(Guid directionId, Guid assigneeId)
    {
        var query = from t in Context.Tasks
                    join d in Context.Directions on t.DirectionId equals d.Id
                    join a in Context.Assignments on t.Id equals a.TaskId
                    where d.Id == directionId && a.AssigneeId == assigneeId
                    select t;
        
        return await query.ToListAsync();
    }
}