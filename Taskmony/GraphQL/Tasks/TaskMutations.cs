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
        [GlobalState] Guid userId, string description, string? details, Guid? assigneeId, Guid? directionId,
        string? startAt)
    {
        var task = new Task
        {
            CreatedById = userId,
            Description = description,
            Details = details,
            AssigneeId = assigneeId,
            DirectionId = directionId,
            StartAt = startAt is null ? null : timeConverter.StringToDateTimeUtc(startAt)
        };

        return await taskService.AddTask(task);
    }

    [Authorize]
    public async Task<Guid[]?> TasksGenerate([Service] ITaskService taskService, [Service] ITimeConverter timeConverter,
        [GlobalState] Guid userId, string description, string? details, Guid? assigneeId, Guid? directionId, string? startAt,
        RepeatMode repeatMode, int? repeatEvery, int numberOfRepetitions)
    {
        var task = new Task
        {
            CreatedById = userId,
            Description = description,
            Details = details,
            AssigneeId = assigneeId,
            DirectionId = directionId,
            StartAt = startAt is null ? null : timeConverter.StringToDateTimeUtc(startAt),
            RepeatMode = repeatMode,
            RepeatEvery = repeatEvery,
            NumberOfRepetitions = numberOfRepetitions
        };

        return await taskService.AddRepeatingTask(task, repeatMode, repeatEvery, numberOfRepetitions);
    }

    [Authorize]
    public async Task<Guid?> TaskSetDescription([Service] ITaskService taskService,
        [GlobalState] Guid userId, Guid taskId, string description)
    {
        if (await taskService.SetTaskDescription(taskId, description, userId))
        {
            return taskId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> TaskSetDetails([Service] ITaskService taskService,
    [GlobalState] Guid userId, Guid taskId, string? details)
    {
        if (await taskService.SetTaskDetails(taskId, details, userId))
        {
            return taskId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> TaskSetDirection([Service] ITaskService taskService,
        [GlobalState] Guid userId, Guid taskId, Guid? directionId)
    {
        if (await taskService.SetTaskDirection(taskId, directionId, userId))
        {
            return taskId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> TaskSetDeletedAt([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid userId, Guid taskId, string? deletedAt)
    {
        DateTime? deletedAtUtc = deletedAt is null ? null : timeConverter.StringToDateTimeUtc(deletedAt);

        if (await taskService.SetTaskDeletedAt(taskId, deletedAtUtc, userId))
        {
            return taskId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> TaskSetAssignee([Service] ITaskService taskService,
        [GlobalState] Guid userId, Guid taskId, Guid? assigneeId)
    {
        if (await taskService.SetTaskAssignee(taskId, assigneeId, userId))
        {
            return taskId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> TaskSetStartAt([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid userId, Guid taskId, string startAt)
    {
        var startAtUtc = timeConverter.StringToDateTimeUtc(startAt);

        if (await taskService.SetTaskStartAt(taskId, startAtUtc, userId))
        {
            return taskId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> TaskSetCompletedAt([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid userId, Guid taskId, string? completedAt)
    {
        DateTime? completedAtUtc = completedAt is null ? null : timeConverter.StringToDateTimeUtc(completedAt);

        if (await taskService.SetTaskCompletedAt(taskId, completedAtUtc, userId))
        {
            return taskId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid[]?> TaskRecurringDeleteAll([Service] ITaskService taskService,
        [GlobalState] Guid userId, Guid groupId)
    {
        return await taskService.SetRecurringTaskDeletedAt(groupId, DateTime.UtcNow, userId);
    }
}