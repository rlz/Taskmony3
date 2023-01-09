namespace Taskmony.Repositories;

public interface ITaskRepository
{
    /// <summary>
    /// Returns user tasks filtered by the given parameters
    /// </summary>
    /// <param name="id">an array of the task ids to filter by</param>
    /// <param name="directionId">an array of the direction ids to filter by</param>
    /// <param name="offset">offset of the tasks sorted by creation date and id</param>
    /// <param name="limit">max number of the tasks sorted by creation date and id to return</param>
    /// <param name="userId">user id</param>
    /// <returns>user tasks</returns>
    Task<IEnumerable<Models.Task>> GetTasksAsync(Guid[]? id, Guid?[] directionId,
        int? offset, int? limit, Guid userId);

    Task<bool> SaveChangesAsync();
}