namespace Taskmony.Services;

public interface ITaskService
{
    /// <summary>
    /// Returns current user tasks filtered by the given parameters
    /// </summary>
    /// <param name="id">an array of the user ids to filter by</param>
    /// <param name="directionId">an array of the direction ids to filter by</param>
    /// <param name="offset">offset of tasks sorted by creation date and id</param>
    /// <param name="limit">max number of the tasks sorted by creation date and id to return</param>
    /// <param name="currentUserId">id of the current user</param>
    /// <returns>current user tasks</returns>
    Task<IEnumerable<Models.Task>> GetTasksAsync(Guid[]? id, Guid?[]? directionId,
        int? offset, int? limit, Guid currentUserId);
}