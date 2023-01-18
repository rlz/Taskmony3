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

    Task<Models.Task> AddTask(Models.Task task);

    Task<Guid[]> AddRepeatingTask(Models.Task task, RepeatMode repeatMode, int? repeatEvery, int numberOfRepetitions);

    Task<bool> SetTaskDescription(Guid id, string description, Guid currentUserId);

    Task<bool> SetTaskDetails(Guid id, string? details, Guid currentUserId);

    Task<bool> SetTaskDirection(Guid id, Guid? directionId, Guid currentUserId);

    Task<bool> SetTaskAssignee(Guid id, Guid? assigneeId, Guid currentUserId);

    Task<bool> SetTaskStartAt(Guid id, DateTime startAtUtc, Guid currentUserId);

    Task<bool> SetTaskDeletedAt(Guid id, DateTime? deletedAtUtc, Guid currentUserId);

    Task<Guid[]> SetRecurringTaskDeletedAt(Guid groupId, DateTime? deletedAtUtc, Guid currentUserId);

    Task<bool> SetTaskCompletedAt(Guid id, DateTime? completedAtUtc, Guid currentUserId);
}