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

    Task<IEnumerable<Models.Task>> GetTasksByIdsAsync(Guid[] ids);

    Task<Models.Task> GetTaskOrThrowAsync(Guid id, Guid currentUserId);

    Task<Models.Task?> AddTaskAsync(Models.Task task);

    Task<IEnumerable<Guid>> AddRecurringTaskAsync(Models.Task task, RepeatMode repeatMode, int? repeatEvery, int numberOfRepetitions);

    Task<IEnumerable<Guid>> SetTaskDescriptionAsync(Guid? taskId, Guid? groupId, string description, Guid currentUserId);

    Task<IEnumerable<Guid>> SetTaskDetailsAsync(Guid? taskId, Guid? groupId, string? details, Guid currentUserId);

    Task<IEnumerable<Guid>> SetTaskDirectionAsync(Guid? taskId, Guid? groupId, Guid? directionId, Guid currentUserId);

    Task<IEnumerable<Guid>> SetTaskAssigneeAsync(Guid? taskId, Guid? groupId, Guid? assigneeId, Guid currentUserId);

    Task<IEnumerable<Guid>> SetTaskStartAtAsync(Guid? taskId, Guid? groupId, DateTime startAtUtc, Guid currentUserId);

    Task<IEnumerable<Guid>> SetTaskDeletedAtAsync(Guid? taskId, Guid? groupId, DateTime? deletedAtUtc, Guid currentUserId);

    Task<Guid?> SetTaskCompletedAtAsync(Guid taskId, DateTime? completedAtUtc, Guid currentUserId);
}