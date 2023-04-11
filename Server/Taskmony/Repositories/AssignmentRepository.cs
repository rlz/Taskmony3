using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models;
using Taskmony.Repositories.Abstract;
using Task = System.Threading.Tasks.Task;

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

    public async Task UpdateAssignmentAsync(Models.Task task, Assignment? newAssignment)
    {
        await using var transaction = await Context.Database.BeginTransactionAsync();

        Context.Attach(task);

        if (task.Assignment != null)
        {
            Context.Remove(task.Assignment);
            await SaveChangesAsync();
        }

        task.Assignment = newAssignment;

        await SaveChangesAsync();

        await transaction.CommitAsync();
    }

    public async Task UpdateAssignmentAsync(IEnumerable<Models.Task> tasks, Assignment? newAssignment)
    {
        await using var transaction = await Context.Database.BeginTransactionAsync();

        foreach (var task in tasks)
        {
            Context.Attach(task);
            
            if (task.Assignment != null)
            {
                Context.Remove(task.Assignment);
                await SaveChangesAsync();
            }

            task.Assignment = newAssignment is null
                ? null
                : new Assignment {AssigneeId = newAssignment.AssigneeId, AssignedById = newAssignment.AssignedById};
        }

        await SaveChangesAsync();

        await transaction.CommitAsync();
    }
}