using HotChocolate.AspNetCore.Authorization;
using Taskmony.Models.Enums;
using Taskmony.Services;
using Task = Taskmony.Models.Task;

namespace Taskmony.GraphQL.Tasks;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class TaskMutations
{
    [Authorize]
    public async Task<Task?> TaskAdd([Service] ITaskService taskService, [Service] ITimeConverter timeConverter,
        [GlobalState] Guid currentUserId, string description, string? details, Guid? assigneeId, Guid? directionId,
        string? startAt)
    {
        var task = new Task
        {
            CreatedById = currentUserId,
            Description = description,
            Details = details,
            AssigneeId = assigneeId,
            DirectionId = directionId,
            StartAt = startAt is null ? null : timeConverter.StringToDateTimeUtc(startAt)
        };

        return await taskService.AddTaskAsync(task);
    }

    [Authorize]
    public async Task<Guid[]?> TasksGenerate([Service] ITaskService taskService, [Service] ITimeConverter timeConverter,
        [GlobalState] Guid currentUserId, string description, string? details, Guid? assigneeId, Guid? directionId, string? startAt,
        RepeatMode repeatMode, int? repeatEvery, int numberOfRepetitions)
    {
        var task = new Task
        {
            CreatedById = currentUserId,
            Description = description,
            Details = details,
            AssigneeId = assigneeId,
            DirectionId = directionId,
            StartAt = startAt is null ? null : timeConverter.StringToDateTimeUtc(startAt),
            RepeatMode = repeatMode,
            RepeatEvery = repeatEvery,
            NumberOfRepetitions = numberOfRepetitions
        };

        return await taskService.AddRepeatingTaskAsync(task, repeatMode, repeatEvery, numberOfRepetitions);
    }

    [Authorize]
    public async Task<Guid?> TaskSetDescription([Service] ITaskService taskService,
        [GlobalState] Guid currentUserId, Guid taskId, string description)
    {
        if (await taskService.SetTaskDescriptionAsync(taskId, description, currentUserId))
        {
            return taskId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> TaskSetDetails([Service] ITaskService taskService,
    [GlobalState] Guid currentUserId, Guid taskId, string? details)
    {
        if (await taskService.SetTaskDetailsAsync(taskId, details, currentUserId))
        {
            return taskId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> TaskSetDirection([Service] ITaskService taskService,
        [GlobalState] Guid currentUserId, Guid taskId, Guid? directionId)
    {
        if (await taskService.SetTaskDirectionAsync(taskId, directionId, currentUserId))
        {
            return taskId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> TaskSetDeletedAt([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid taskId, string? deletedAt)
    {
        DateTime? deletedAtUtc = deletedAt is null ? null : timeConverter.StringToDateTimeUtc(deletedAt);

        if (await taskService.SetTaskDeletedAtAsync(taskId, deletedAtUtc, currentUserId))
        {
            return taskId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> TaskSetAssignee([Service] ITaskService taskService,
        [GlobalState] Guid currentUserId, Guid taskId, Guid? assigneeId)
    {
        if (await taskService.SetTaskAssigneeAsync(taskId, assigneeId, currentUserId))
        {
            return taskId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> TaskSetStartAt([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid taskId, string startAt)
    {
        var startAtUtc = timeConverter.StringToDateTimeUtc(startAt);

        if (await taskService.SetTaskStartAtAsync(taskId, startAtUtc, currentUserId))
        {
            return taskId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> TaskSetCompletedAt([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid taskId, string? completedAt)
    {
        DateTime? completedAtUtc = completedAt is null ? null : timeConverter.StringToDateTimeUtc(completedAt);

        if (await taskService.SetTaskCompletedAtAsync(taskId, completedAtUtc, currentUserId))
        {
            return taskId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid[]?> TaskRecurringDeleteAll([Service] ITaskService taskService,
        [GlobalState] Guid currentUserId, Guid groupId)
    {
        return await taskService.SetRecurringTaskDeletedAtAsync(groupId, DateTime.UtcNow, currentUserId);
    }
}