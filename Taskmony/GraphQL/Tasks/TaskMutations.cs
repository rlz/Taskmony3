using HotChocolate.AspNetCore.Authorization;
using Taskmony.Errors;
using Taskmony.Exceptions;
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
    public async Task<IEnumerable<Guid>?> TasksGenerate([Service] ITaskService taskService, [Service] ITimeConverter timeConverter,
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

        return await taskService.AddRecurringTaskAsync(task, repeatMode, repeatEvery, numberOfRepetitions);
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetDescription([Service] ITaskService taskService,
        [GlobalState] Guid currentUserId, Guid? taskId, Guid? groupId, string description)
    {
        return await taskService.SetTaskDescriptionAsync(taskId, groupId, description, currentUserId);
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetDetails([Service] ITaskService taskService,
    [GlobalState] Guid currentUserId, Guid? taskId, Guid? groupId, string? details)
    {
        return await taskService.SetTaskDetailsAsync(taskId, groupId, details, currentUserId);
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetDirection([Service] ITaskService taskService,
        [GlobalState] Guid currentUserId, Guid? taskId, Guid? groupId, Guid? directionId)
    {
        return await taskService.SetTaskDirectionAsync(taskId, groupId, directionId, currentUserId);
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetDeletedAt([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId,
        Guid? taskId, Guid? groupId, string? deletedAt)
    {
        DateTime? deletedAtUtc = deletedAt is null ? null : timeConverter.StringToDateTimeUtc(deletedAt);

        return await taskService.SetTaskDeletedAtAsync(taskId, groupId, deletedAtUtc, currentUserId);
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetAssignee([Service] ITaskService taskService,
        [GlobalState] Guid currentUserId, Guid? taskId, Guid? groupId, Guid? assigneeId)
    {
        return await taskService.SetTaskAssigneeAsync(taskId, groupId, assigneeId, currentUserId);
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetStartAt([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid? taskId, Guid? groupId, string startAt)
    {
        var startAtUtc = timeConverter.StringToDateTimeUtc(startAt);

        return await taskService.SetTaskStartAtAsync(taskId, groupId, startAtUtc, currentUserId);
    }

    [Authorize]
    public async Task<Guid?> TaskSetCompletedAt([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid taskId, string? completedAt)
    {
        DateTime? completedAtUtc = completedAt is null ? null : timeConverter.StringToDateTimeUtc(completedAt);

        return await taskService.SetTaskCompletedAtAsync(taskId, completedAtUtc, currentUserId);
    }
}