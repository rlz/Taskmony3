using Taskmony.Models.Enums;

namespace Taskmony.Services.Abstract;

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

    Task<IEnumerable<Models.Task>> GetTasksByIdsAsync(IEnumerable<Guid> ids);

    Task<Models.Task> GetTaskOrThrowAsync(Guid id, Guid currentUserId);

    Task<Models.Task?> AddTaskAsync(Models.Task task);

    Task<IEnumerable<Guid>> AddRecurringTaskAsync(Models.Task task, RepeatMode repeatMode,
        int repeatEvery, WeekDay? weekDays, DateTime repeatUntil);

    Task<IEnumerable<Guid>> SetRecurringTaskDescriptionAsync(Guid groupId, string description, Guid currentUserId);

    Task<Guid?> SetTaskDescriptionAsync(Guid taskId, string description, Guid currentUserId);

    Task<IEnumerable<Guid>> SetRecurringTaskDetailsAsync(Guid groupId, string? details, Guid currentUserId);

    Task<Guid?> SetTaskDetailsAsync(Guid taskId, string? details, Guid currentUserId);

    Task<IEnumerable<Guid>> SetRecurringTaskDirectionAsync(Guid groupId, Guid? directionId, Guid currentUserId);

    Task<Guid?> SetTaskDirectionAsync(Guid taskId, Guid? directionId, Guid currentUserId);

    Task<IEnumerable<Guid>> SetRecurringTaskAssigneeAsync(Guid groupId, Guid? assigneeId, Guid currentUserId);

    Task<Guid?> SetTaskAssigneeAsync(Guid taskId, Guid? assigneeId, Guid currentUserId);

    Task<IEnumerable<Guid>> SetRecurringTaskStartAtAsync(Guid groupId, DateTime startAtUtc, Guid currentUserId);

    Task<Guid?> SetTaskStartAtAsync(Guid taskId, DateTime startAtUtc, Guid currentUserId);

    Task<IEnumerable<Guid>> SetRecurringTaskDeletedAtAsync(Guid groupId, DateTime? deletedAtUtc, Guid currentUserId, bool all);

    Task<Guid?> SetTaskDeletedAtAsync(Guid taskId, DateTime? deletedAtUtc, Guid currentUserId);

    Task<Guid?> SetTaskCompletedAtAsync(Guid taskId, DateTime? completedAtUtc, Guid currentUserId);

    Task<IEnumerable<Guid>> SetRecurringTaskRepeatModeAsync(Guid groupId, RepeatMode? repeatMode, 
        WeekDay? weekDays, DateTime? repeatUntil, int? repeatEvery, Guid currentUserId);

    Task<IEnumerable<Guid>> SetTaskRepeatModeAsync(Guid taskId, RepeatMode? repeatMode, WeekDay? weekDays, 
        DateTime? repeatUntil, int? repeatEvery, Guid currentUserId);

    Task<IEnumerable<Guid>> SetRecurringTaskRepeatUntilAsync(Guid groupId, DateTime repeatUntil, Guid currentUserId);
}