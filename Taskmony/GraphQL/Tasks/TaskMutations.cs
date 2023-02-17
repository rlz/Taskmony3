using HotChocolate.AspNetCore.Authorization;
using Taskmony.Models.Enums;
using Taskmony.Services.Abstract;
using Taskmony.ValueObjects;
using Task = Taskmony.Models.Task;

namespace Taskmony.GraphQL.Tasks;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class TaskMutations
{
    [Authorize]
    public async Task<Task?> TaskAdd([Service] ITaskService taskService, [Service] ITimeConverter timeConverter,
        [GlobalState] Guid currentUserId, string description, string? details, Guid? assigneeId, Guid? directionId,
        string startAt)
    {
        var task = new Task
        {
            CreatedById = currentUserId,
            Description = Description.From(description),
            Details = details,
            AssigneeId = assigneeId,
            DirectionId = directionId,
            StartAt = timeConverter.StringToDateTimeUtc(startAt)
        };

        return await taskService.AddTaskAsync(task);
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TasksGenerate([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, string description,
        string? details, Guid? assigneeId, Guid? directionId, string startAt, RepeatMode repeatMode,
        int repeatEvery, WeekDay[]? weekDays, string repeatUntil)
    {
        var repeatUntilUtc = timeConverter.StringToDateTimeUtc(repeatUntil);

        var task = new Task
        {
            CreatedById = currentUserId,
            Description = Description.From(description),
            Details = details,
            AssigneeId = assigneeId,
            DirectionId = directionId,
            StartAt = timeConverter.StringToDateTimeUtc(startAt),
            RepeatMode = repeatMode,
            RepeatEvery = repeatEvery,
            WeekDays = weekDays?.Aggregate((current, weekDay) => current | weekDay),
            RepeatUntil = RepeatUntil.From(repeatUntilUtc)
        };

        return await taskService.AddRecurringTaskAsync(task, repeatMode, repeatEvery, task.WeekDays, repeatUntilUtc);
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetDescription([Service] ITaskService taskService,
        [GlobalState] Guid currentUserId, Guid taskId, Guid? groupId, string description)
    {
        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskDescriptionAsync(groupId.Value, description, currentUserId);
        }

        return await taskService.SetTaskDescriptionAsync(taskId, description, currentUserId) is null
            ? null
            : new[] { taskId };
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetDetails([Service] ITaskService taskService,
        [GlobalState] Guid currentUserId, Guid taskId, Guid? groupId, string? details)
    {
        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskDetailsAsync(groupId.Value, details, currentUserId);
        }

        return await taskService.SetTaskDetailsAsync(taskId, details, currentUserId) is null
            ? null
            : new[] { taskId };
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetDirection([Service] ITaskService taskService,
        [GlobalState] Guid currentUserId, Guid taskId, Guid? groupId, Guid? directionId)
    {
        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskDirectionAsync(groupId.Value, directionId, currentUserId);
        }

        return await taskService.SetTaskDirectionAsync(taskId, directionId, currentUserId) is null
            ? null
            : new[] { taskId };
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetDeletedAt([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid taskId, 
        Guid? groupId, string? deletedAt, bool? all)
    {
        DateTime? deletedAtUtc = deletedAt is null ? null : timeConverter.StringToDateTimeUtc(deletedAt);

        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskDeletedAtAsync(groupId.Value, deletedAtUtc, currentUserId, all ?? false);
        }

        return await taskService.SetTaskDeletedAtAsync(taskId, deletedAtUtc, currentUserId) is null
            ? null
            : new[] { taskId };
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetAssignee([Service] ITaskService taskService,
        [GlobalState] Guid currentUserId, Guid taskId, Guid? groupId, Guid? assigneeId)
    {
        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskAssigneeAsync(groupId.Value, assigneeId, currentUserId);
        }

        return await taskService.SetTaskAssigneeAsync(taskId, assigneeId, currentUserId) is null
            ? null
            : new[] { taskId };
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetStartAt([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid taskId, 
        Guid? groupId, string startAt)
    {
        var startAtUtc = timeConverter.StringToDateTimeUtc(startAt);

        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskStartAtAsync(groupId.Value, startAtUtc, currentUserId);
        }

        return await taskService.SetTaskStartAtAsync(taskId, startAtUtc, currentUserId) is null
            ? null
            : new[] { taskId };
    }

    [Authorize]
    public async Task<Guid?> TaskSetCompletedAt([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid taskId, string? completedAt)
    {
        DateTime? completedAtUtc = completedAt is null ? null : timeConverter.StringToDateTimeUtc(completedAt);

        return await taskService.SetTaskCompletedAtAsync(taskId, completedAtUtc, currentUserId);
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetRepeatMode([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid taskId, 
        Guid? groupId, RepeatMode? repeatMode, WeekDay[]? weekDays, string? repeatUntil, int? repeatEvery)
    {
        DateTime? repeatUntilUtc = repeatUntil == null ? null : timeConverter.StringToDateTimeUtc(repeatUntil);
        var weekDayFlags = weekDays?.Aggregate((current, weekDay) => current | weekDay);

        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskRepeatModeAsync(groupId.Value, repeatMode, weekDayFlags,
                repeatUntilUtc, repeatEvery, currentUserId);
        }

        return await taskService.SetTaskRepeatModeAsync(taskId, repeatMode, weekDayFlags, repeatUntilUtc,
            repeatEvery, currentUserId);
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetRepeatUntil([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid groupId, string repeatUntil)
    {
        var repeatUntilUtc = timeConverter.StringToDateTimeUtc(repeatUntil);

        return await taskService.SetRecurringTaskRepeatUntilAsync(groupId, repeatUntilUtc, currentUserId);
    }
}