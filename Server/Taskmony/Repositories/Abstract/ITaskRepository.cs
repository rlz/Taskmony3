namespace Taskmony.Repositories.Abstract;

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
    Task<IEnumerable<Models.Tasks.Task>> GetAsync(Guid[]? id, Guid?[] directionId, int? offset,
        int? limit, Guid userId);

    Task<IEnumerable<Models.Tasks.Task>> GetByIdsAsync(IEnumerable<Guid> ids);

    Task<Models.Tasks.Task?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets not completed and not deleted tasks
    /// </summary>
    /// <param name="groupId">group id of the recurring task</param>
    /// <returns>active tasks from the group</returns>
    Task<IEnumerable<Models.Tasks.Task>> GetActiveTasksAsync(Guid groupId);

    Task<IEnumerable<Models.Tasks.Task>> GetTasksByGroupIdAsync(Guid groupId);

    Task<IEnumerable<Models.Tasks.Task>> GetByDirectionIdAndAssigneeIdAsync(Guid directionId, Guid assigneeId);

    Task SoftDeleteDirectionTasksAndCommentsAsync(Guid directionId);

    Task UndeleteDirectionTasksAndComments(Guid directionId, DateTime deletedAt);

    Task AddAsync(Models.Tasks.Task task);

    Task AddRangeAsync(IEnumerable<Models.Tasks.Task> tasks);

    void DeleteRange(IEnumerable<Models.Tasks.Task> tasks);

    void Delete(Models.Tasks.Task task);

    Task<bool> SaveChangesAsync();
}