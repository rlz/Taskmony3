using Taskmony.Models.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Services.Abstract;

public interface ITaskService
{
    /// <summary>
    /// Gets current user tasks filtered by the given parameters. If both completed and deleted are null, returns all
    /// </summary>
    /// <param name="id">an array of the user ids to filter by</param>
    /// <param name="directionId">an array of the direction ids to filter by</param>
    /// <param name="lastDeletedAt">time of last deleted task</param>
    /// <param name="offset">offset of tasks sorted by creation date and id</param>
    /// <param name="limit">max number of the tasks sorted by creation date and id to return</param>
    /// <param name="currentUserId">id of the current user</param>
    /// <param name="deleted">whether tasks are deleted</param>
    /// <param name="lastCompletedAt">time of last completed task</param>
    /// <param name="completed">whether tasks are completed</param>
    /// <returns>current user tasks</returns>
    Task<IEnumerable<Models.Tasks.Task>> GetTasksAsync(Guid[]? id, Guid?[]? directionId, bool? completed, bool? deleted,
        DateTime? lastCompletedAt, DateTime? lastDeletedAt, int? offset, int? limit, Guid currentUserId);

    Task<IEnumerable<Models.Tasks.Task>> GetTasksByIdsAsync(IEnumerable<Guid> ids);

    Task<Models.Tasks.Task> GetTaskOrThrowAsync(Guid id, Guid currentUserId);

    Task<Models.Tasks.Task?> AddTaskAsync(string description, string? details, Guid? assigneeId, Guid? directionId,
        DateTime startAtUtc, Guid currentUserId);

    Task<IEnumerable<Guid>> AddRecurringTaskAsync(string description, string? details, Guid? assigneeId,
        Guid? directionId, DateTime startAtUtc, RepeatMode repeatMode, int repeatEvery, WeekDay? weekDays,
        DateTime repeatUntilUtc, Guid currentUserId);

    Task<IEnumerable<Guid>> SetRecurringTaskDescriptionAsync(Guid taskId, Guid groupId,
        string description, Guid currentUserId);

    Task<Guid?> SetTaskDescriptionAsync(Guid taskId, string description, Guid currentUserId);

    Task<IEnumerable<Guid>> SetRecurringTaskDetailsAsync(Guid taskId, Guid groupId,
        string? details, Guid currentUserId);

    Task<Guid?> SetTaskDetailsAsync(Guid taskId, string? details, Guid currentUserId);

    Task<IEnumerable<Guid>> SetRecurringTaskDirectionAsync(Guid taskId, Guid groupId,
        Guid? directionId, Guid currentUserId);

    Task<Guid?> SetTaskDirectionAsync(Guid taskId, Guid? directionId, Guid currentUserId);

    Task<IEnumerable<Guid>> SetRecurringTaskAssigneeAsync(Guid taskId, Guid groupId, Guid? assigneeId,
        Guid currentUserId);

    Task<Guid?> SetTaskAssigneeAsync(Guid taskId, Guid? assigneeId, Guid currentUserId);

    Task<IEnumerable<Guid>> SetRecurringTaskStartAtAsync(Guid taskId, Guid groupId, DateTime startAtUtc,
        Guid currentUserId);

    Task<Guid?> SetTaskStartAtAsync(Guid taskId, DateTime startAtUtc, Guid currentUserId);

    Task<IEnumerable<Guid>> SetRecurringTaskDeletedAtAsync(Guid taskId, Guid groupId, DateTime? deletedAtUtc,
        Guid currentUserId, bool all);

    Task<Guid?> SetTaskDeletedAtAsync(Guid taskId, DateTime? deletedAtUtc, Guid currentUserId);

    Task<Guid?> SetTaskCompletedAtAsync(Guid taskId, DateTime? completedAtUtc, Guid currentUserId);

    Task<IEnumerable<Guid>> SetRecurringTaskRepeatModeAsync(Guid taskId, Guid groupId, RepeatMode? repeatMode,
        WeekDay? weekDays, DateTime? startAt, DateTime? repeatUntil, int? repeatEvery, Guid currentUserId);

    Task<IEnumerable<Guid>> SetTaskRepeatModeAsync(Guid taskId, RepeatMode? repeatMode, WeekDay? weekDays,
        DateTime? startAt, DateTime? repeatUntil, int? repeatEvery, Guid currentUserId);

    Task<IEnumerable<Guid>> SetRecurringTaskRepeatUntilAsync(Guid taskId, Guid groupId, DateTime repeatUntil,
        Guid currentUserId);

    Task RemoveAssigneeFromDirectionTasksAsync(Guid assigneeId, Guid directionId);

    Task SoftDeleteDirectionTasksAsync(Guid directionId);

    Task UndeleteDirectionTasksAsync(Guid directionId, DateTime deletedAt);
}