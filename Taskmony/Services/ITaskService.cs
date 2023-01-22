using Taskmony.Models.Enums;

namespace Taskmony.Services;

public interface ITaskService
{
    /// <summary>
    /// Gets current user tasks filtered by the given parameters
    /// </summary>
    /// <param name="id">an array of the user ids to filter by</param>
    /// <param name="directionId">an array of the direction ids to filter by</param>
    /// <param name="offset">offset of tasks sorted by creation date and id</param>
    /// <param name="limit">max number of the tasks sorted by creation date and id to return</param>
    /// <param name="currentUserId">id of the current user</param>
    /// <returns>current user tasks</returns>
    Task<IEnumerable<Models.Task>> GetTasksAsync(Guid[]? id, Guid?[]? directionId,
        int? offset, int? limit, Guid currentUserId);

    Task<Models.Task> GetTaskOrThrowAsync(Guid id, Guid currentUserId);

    Task<Models.Task> AddTaskAsync(Models.Task task);

    Task<Guid[]> AddRepeatingTaskAsync(Models.Task task, RepeatMode repeatMode, int? repeatEvery, int numberOfRepetitions);

    Task<bool> SetTaskDescriptionAsync(Guid id, string description, Guid currentUserId);

    Task<bool> SetTaskDetailsAsync(Guid id, string? details, Guid currentUserId);

    Task<bool> SetTaskDirectionAsync(Guid id, Guid? directionId, Guid currentUserId);

    Task<bool> SetTaskAssigneeAsync(Guid id, Guid? assigneeId, Guid currentUserId);

    Task<bool> SetTaskStartAtAsync(Guid id, DateTime startAtUtc, Guid currentUserId);

    Task<bool> SetTaskDeletedAtAsync(Guid id, DateTime? deletedAtUtc, Guid currentUserId);

    Task<Guid[]> SetRecurringTaskDeletedAtAsync(Guid groupId, DateTime? deletedAtUtc, Guid currentUserId);

    Task<bool> SetTaskCompletedAtAsync(Guid id, DateTime? completedAtUtc, Guid currentUserId);
}