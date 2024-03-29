using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models.ValueObjects;
using Taskmony.Repositories.Abstract;

namespace Taskmony.Repositories;

public sealed class TaskRepository : BaseRepository<Models.Tasks.Task>, ITaskRepository
{
    public TaskRepository(IDbContextFactory<TaskmonyDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<IEnumerable<Models.Tasks.Task>> GetAsync(Guid[]? id, Guid?[] directionId, bool? completed,
        bool? deleted, DateTime? lastCompletedAt, DateTime? lastDeletedAt, int? offset, int? limit, Guid userId)
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

        if (deleted != null)
        {
            query = deleted.Value
                ? query.Where(t => t.DeletedAt != null && (lastDeletedAt == null || t.DeletedAt.Value <= lastDeletedAt))
                : query.Where(t => t.DeletedAt == null);
        }

        if (completed != null)
        {
            query = completed.Value
                ? query.Where(t =>
                    t.CompletedAt != null && (lastCompletedAt == null || t.CompletedAt.Value <= lastCompletedAt))
                : query.Where(t => t.CompletedAt == null);
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

    public async Task DeleteUserAssignmentsInDirectionAsync(Guid directionId,
        Guid assigneeId)
    {
        var toDelete = from t in Context.Tasks
                       where t.DirectionId == directionId && t.DeletedAt == null && t.CompletedAt == null
                       join a in Context.Assignments on t.Id equals a.TaskId
                       where a.AssigneeId == assigneeId
                       select a;

        await toDelete.ExecuteDeleteAsync();
    }

    public async Task SoftDeleteDirectionTasksAndCommentsAsync(Guid directionId)
    {
        var now = DateTime.UtcNow;

        var comments = from t in Context.Tasks
                       where t.DirectionId == directionId && t.DeletedAt == null
                       from c in Context.TaskComments.Where(c => c.TaskId == t.Id && c.DeletedAt == null)
                       select c;

        var tasks = from t in Context.Tasks
                    where t.DirectionId == directionId && t.DeletedAt == null && t.CompletedAt == null
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
                       from c in Context.TaskComments.Where(c =>
                           c.TaskId == t.Id && c.DeletedAt != null && c.DeletedAt.Value >= deletedAt)
                       select c;

        await tasks.ForEachAsync(t => t.UpdateDeletedAt(null));
        await comments.ForEachAsync(c => c.UpdateDeletedAt(null));

        await Context.SaveChangesAsync();
    }

    public async Task HardDeleteSoftDeletedTasksWithChildrenAsync(DateTime deletedBeforeOrAt)
    {
        // Comments are deleted with cascade
        await Context.Tasks
            .Where(t => t.DeletedAt != null && t.DeletedAt.Value <= deletedBeforeOrAt)
            .ExecuteDeleteAsync();
    }
}