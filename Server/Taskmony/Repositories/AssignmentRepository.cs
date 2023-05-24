using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models.Tasks;
using Taskmony.Repositories.Abstract;
using Task = Taskmony.Models.Tasks.Task;

namespace Taskmony.Repositories;

public class AssignmentRepository : BaseRepository<Assignment>, IAssignmentRepository
{
    public AssignmentRepository(IDbContextFactory<TaskmonyDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<IEnumerable<Assignment>> GetByTaskIdsAsync(IEnumerable<Guid> taskIds)
    {
        return await Context.Assignments.Where(a => taskIds.Contains(a.TaskId)).ToListAsync();
    }

    public async Task<bool> UpdateAssignmentAsync(Task task, Assignment? newAssignment)
    {
        var anyChanges = false;
        await using var transaction = await Context.Database.BeginTransactionAsync();

        Context.Attach(task);

        if (task.Assignment != null)
        {
            Context.Remove(task.Assignment);
            anyChanges = await SaveChangesAsync();
        }

        task.UpdateAssignment(newAssignment);

        if (task.GroupId != null)
        {
            task.RemoveFromGroup();
        }

        anyChanges |= await SaveChangesAsync();

        await transaction.CommitAsync();

        return anyChanges;
    }

    public async Task<bool> UpdateAssignmentAsync(IEnumerable<Task> tasks, Assignment? newAssignment)
    {
        var anyChanges = false;
        await using var transaction = await Context.Database.BeginTransactionAsync();

        foreach (var task in tasks)
        {
            Context.Attach(task);

            if (task.Assignment != null)
            {
                Context.Remove(task.Assignment);
                anyChanges = await SaveChangesAsync();
            }

            task.UpdateAssignment(newAssignment == null
                ? null
                : new Assignment(newAssignment.AssigneeId, newAssignment.AssignedById));
        }

        anyChanges |= await SaveChangesAsync();

        await transaction.CommitAsync();

        return anyChanges;
    }
}