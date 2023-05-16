using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Repositories.Abstract;
using Taskmony.ValueObjects;

namespace Taskmony.Repositories;

public sealed class TaskRepository : BaseRepository<Models.Tasks.Task>, ITaskRepository
{
    public TaskRepository(IDbContextFactory<TaskmonyDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<IEnumerable<Models.Tasks.Task>> GetAsync(Guid[]? id, Guid?[] directionId, int? offset,
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

        return await query.Include(t => t.RecurrencePattern).ToListAsync();
    }

    private IQueryable<Models.Tasks.Task> AddPagination(IQueryable<Models.Tasks.Task> query, int? offset, int? limit)
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

    public new async Task<Models.Tasks.Task?> GetByIdAsync(Guid id)
    {
        return await Context.Tasks
            .Include(t => t.Assignment)
            .Include(t => t.RecurrencePattern)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Models.Tasks.Task>> GetActiveTasksAsync(Guid groupId)
    {
        return await Context.Tasks
            .Where(t => t.GroupId == groupId && t.CompletedAt == null && t.DeletedAt == null)
            .Include(t => t.Assignment)
            .Include(t => t.RecurrencePattern)
            .ToListAsync();
    }

    public async Task<IEnumerable<Models.Tasks.Task>> GetTasksByGroupIdAsync(Guid groupId)
    {
        return await Context.Tasks.Where(t => t.GroupId == groupId).ToListAsync();
    }

    public async Task<IEnumerable<Models.Tasks.Task>> GetByDirectionIdAndAssigneeIdAsync(Guid directionId, Guid assigneeId)
    {
        var query = from t in Context.Tasks
                    join d in Context.Directions on t.DirectionId equals d.Id
                    join a in Context.Assignments on t.Id equals a.TaskId
                    where d.Id == directionId && a.AssigneeId == assigneeId
                    select t;

        return await query.ToListAsync();
    }

    public async Task SoftDeleteDirectionTasksAndCommentsAsync(Guid directionId)
    {
        var now = DateTime.UtcNow;

        var comments = from t in Context.Tasks
                       where t.DirectionId == directionId && t.DeletedAt == null
                       from c in Context.TaskComments.Where(c => c.TaskId == t.Id && c.DeletedAt == null)
                       select c;

        var tasks = from t in Context.Tasks
                    where t.DirectionId == directionId && t.DeletedAt == null
                    select t;

        await tasks.ForEachAsync(t => t.UpdateDeletedAt(DeletedAt.From(now)));
        await comments.ForEachAsync(c => c.UpdateDeletedAt(DeletedAt.From(now)));

        await Context.SaveChangesAsync();
    }

    public async Task UndeleteDirectionTasksAndComments(Guid directionId, DateTime deletedAt)
    {
        var tasks = from t in Context.Tasks
                    where t.DirectionId == directionId && t.DeletedAt != null && t.DeletedAt.Value >= deletedAt
                    select t;

        var comments = from t in Context.Tasks
                       where t.DirectionId == directionId && t.DeletedAt != null && t.DeletedAt.Value >= deletedAt
                       from c in Context.TaskComments.Where(c => c.TaskId == t.Id && c.DeletedAt != null && c.DeletedAt.Value >= deletedAt)
                       select c;

        await tasks.ForEachAsync(t => t.UpdateDeletedAt(null));
        await comments.ForEachAsync(c => c.UpdateDeletedAt(null));

        await Context.SaveChangesAsync();
    }
}