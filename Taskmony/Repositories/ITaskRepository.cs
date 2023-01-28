namespace Taskmony.Repositories;

public interface ITaskRepository
{
    /// <summary>
    /// Gets user tasks filtered by the given parameters
    /// </summary>
    /// <param name="id">an array of the task ids to filter by</param>
    /// <param name="directionId">an array of the direction ids to filter by</param>
    /// <param name="offset">offset of the tasks sorted by creation date and id</param>
    /// <param name="limit">max number of the tasks sorted by creation date and id to return</param>
    /// <param name="userId">user id</param>
    /// <returns>user tasks</returns>
    Task<IEnumerable<Models.Task>> GetTasksAsync(Guid[]? id, Guid?[] directionId,
        int? offset, int? limit, Guid userId);

    Task<IEnumerable<Models.Task>> GetTasksByIdsAsync(Guid[] ids);

    Task<Models.Task?> GetTaskByIdAsync(Guid id);

    Task<IEnumerable<Models.Task>> GetNotCompletedTasksAsync(Guid groupId);

    /// <summary>
    /// Gets not completed and not deleted tasks by group id
    /// </summary>
    /// <param name="groupId">group id of the recurring task</param>
    /// <returns>active tasks from the group</returns>
    Task<IEnumerable<Models.Task>> GetActiveTasksAsync(Guid groupId);

    Task AddTaskAsync(Models.Task task);

    Task AddTasksAsync(IReadOnlyCollection<Models.Task> tasks);

    Task<bool> SaveChangesAsync();
}