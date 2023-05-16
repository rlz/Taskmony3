using Taskmony.Models.Tasks;

namespace Taskmony.Repositories.Abstract;

public interface IAssignmentRepository
{
    Task<bool> UpdateAssignmentAsync(Models.Tasks.Task task, Assignment? newAssignment);

    Task<bool> UpdateAssignmentAsync(IEnumerable<Models.Tasks.Task> tasks, Assignment? newAssignment);

    Task<IEnumerable<Assignment>> GetByTaskIdsAsync(IEnumerable<Guid> taskIds);
}