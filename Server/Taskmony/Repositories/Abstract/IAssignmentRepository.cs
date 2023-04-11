using Taskmony.Models;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Repositories.Abstract;

public interface IAssignmentRepository
{
    Task<Assignment?> GetByIdAsync(Guid id);

    Task UpdateAssignmentAsync(Models.Task task, Assignment? newAssignment);

    Task UpdateAssignmentAsync(IEnumerable<Models.Task> tasks, Assignment? newAssignment);

    Task<IEnumerable<Assignment>> GetByTaskIdsAsync(IEnumerable<Guid> taskIds);

    void Delete(Assignment assignment);
}