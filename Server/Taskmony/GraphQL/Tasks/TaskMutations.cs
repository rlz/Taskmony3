using Taskmony.Models.Tasks;
using Taskmony.Services.Abstract;
using Task = Taskmony.Models.Tasks.Task;

namespace Taskmony.GraphQL.Tasks;

[ExtendObjectType(typeof(Mutation))]
public class TaskMutations
{
    public async Task<Task?> TaskAdd([Service] ITaskService taskService, [Service] ITimeConverter timeConverter,
        [GlobalState] Guid currentUserId, string description, string? details, Guid? assigneeId, Guid? directionId,
        string startAt)
    {
        var startAtUtc = timeConverter.StringToDateTimeUtc(startAt);

        return await taskService.AddTaskAsync(description, details, assigneeId, directionId, startAtUtc, currentUserId);
    }

    public async Task<IEnumerable<Guid>?> TasksGenerate([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, string description,
        string? details, Guid? assigneeId, Guid? directionId, string startAt, RepeatMode repeatMode,
        int repeatEvery, [GraphQLType<ListType<NonNullType<EnumType<WeekDay>>>>] WeekDay? weekDays, string repeatUntil)
    {
        var startAtUtc = timeConverter.StringToDateTimeUtc(startAt);
        var repeatUntilUtc = timeConverter.StringToDateTimeUtc(repeatUntil);

        return await taskService.AddRecurringTaskAsync(description, details, assigneeId, directionId, startAtUtc,
            repeatMode, repeatEvery, weekDays, repeatUntilUtc, currentUserId);
    }

    public async Task<IEnumerable<Guid>?> TaskSetDescription([Service] ITaskService taskService,
        [GlobalState] Guid currentUserId, Guid taskId, Guid? groupId, string description)
    {
        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskDescriptionAsync(taskId, groupId.Value, description,
                currentUserId);
        }

        return await taskService.SetTaskDescriptionAsync(taskId, description, currentUserId) is null
            ? null
            : new[] {taskId};
    }

    public async Task<IEnumerable<Guid>?> TaskSetDetails([Service] ITaskService taskService,
        [GlobalState] Guid currentUserId, Guid taskId, Guid? groupId, string? details)
    {
        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskDetailsAsync(taskId, groupId.Value, details, currentUserId);
        }

        return await taskService.SetTaskDetailsAsync(taskId, details, currentUserId) is null
            ? null
            : new[] {taskId};
    }

    public async Task<IEnumerable<Guid>?> TaskSetDirection([Service] ITaskService taskService,
        [GlobalState] Guid currentUserId, Guid taskId, Guid? groupId, Guid? directionId)
    {
        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskDirectionAsync(taskId, groupId.Value, directionId, currentUserId);
        }

        return await taskService.SetTaskDirectionAsync(taskId, directionId, currentUserId) is null
            ? null
            : new[] {taskId};
    }

    public async Task<IEnumerable<Guid>?> TaskSetDeletedAt([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid taskId,
        Guid? groupId, string? deletedAt, bool? all)
    {
        DateTime? deletedAtUtc = deletedAt is null ? null : timeConverter.StringToDateTimeUtc(deletedAt);

        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskDeletedAtAsync(taskId, groupId.Value, deletedAtUtc, currentUserId,
                all ?? false);
        }

        return await taskService.SetTaskDeletedAtAsync(taskId, deletedAtUtc, currentUserId) is null
            ? null
            : new[] {taskId};
    }

    public async Task<IEnumerable<Guid>?> TaskSetAssignee([Service] ITaskService taskService,
        [GlobalState] Guid currentUserId, Guid taskId, Guid? groupId, Guid? assigneeId)
    {
        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskAssigneeAsync(taskId, groupId.Value, assigneeId, currentUserId);
        }

        return await taskService.SetTaskAssigneeAsync(taskId, assigneeId, currentUserId) is null
            ? null
            : new[] {taskId};
    }

    public async Task<IEnumerable<Guid>?> TaskSetStartAt([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid taskId,
        Guid? groupId, string startAt)
    {
        var startAtUtc = timeConverter.StringToDateTimeUtc(startAt);

        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskStartAtAsync(taskId, groupId.Value, startAtUtc, currentUserId);
        }

        return await taskService.SetTaskStartAtAsync(taskId, startAtUtc, currentUserId) is null
            ? null
            : new[] {taskId};
    }

    public async Task<Guid?> TaskSetCompletedAt([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid taskId, string? completedAt)
    {
        DateTime? completedAtUtc = completedAt is null ? null : timeConverter.StringToDateTimeUtc(completedAt);

        return await taskService.SetTaskCompletedAtAsync(taskId, completedAtUtc, currentUserId);
    }

    public async Task<IEnumerable<Guid>?> TaskSetRepeatMode([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid taskId, Guid? groupId,
        RepeatMode? repeatMode, [GraphQLType<ListType<NonNullType<EnumType<WeekDay>>>>] WeekDay? weekDays,
        string? startAt, string? repeatUntil, int? repeatEvery)
    {
        DateTime? repeatUntilUtc = repeatUntil == null ? null : timeConverter.StringToDateTimeUtc(repeatUntil);
        DateTime? startAtUtc = startAt == null ? null : timeConverter.StringToDateTimeUtc(startAt);

        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskRepeatModeAsync(taskId, groupId.Value, repeatMode, weekDays,
                startAtUtc, repeatUntilUtc, repeatEvery, currentUserId);
        }

        return await taskService.SetTaskRepeatModeAsync(taskId, repeatMode, weekDays, startAtUtc,
            repeatUntilUtc, repeatEvery, currentUserId);
    }

    public async Task<IEnumerable<Guid>?> TaskSetRepeatUntil([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid taskId,
        Guid groupId, string repeatUntil)
    {
        var repeatUntilUtc = timeConverter.StringToDateTimeUtc(repeatUntil);

        return await taskService.SetRecurringTaskRepeatUntilAsync(taskId, groupId, repeatUntilUtc, currentUserId);
    }
}