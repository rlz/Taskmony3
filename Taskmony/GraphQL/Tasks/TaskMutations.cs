using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Resolvers;
using Taskmony.Errors;
using Taskmony.Models.Enums;
using Taskmony.Services;
using Taskmony.ValueObjects;
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
            Description = Description.From(description),
            Details = details,
            AssigneeId = assigneeId,
            DirectionId = directionId,
            StartAt = startAt is null ? null : StartAt.From(timeConverter.StringToDateTimeUtc(startAt))
        };

        return await taskService.AddTaskAsync(task);
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TasksGenerate([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, string description, string? details,
        Guid? assigneeId, Guid? directionId, string? startAt, RepeatMode repeatMode, int? repeatEvery,
        int numberOfRepetitions)
    {
        var task = new Task
        {
            CreatedById = currentUserId,
            Description = Description.From(description),
            Details = details,
            AssigneeId = assigneeId,
            DirectionId = directionId,
            StartAt = startAt is null ? null : StartAt.From(timeConverter.StringToDateTimeUtc(startAt)),
            RepeatMode = repeatMode,
            RepeatEvery = repeatEvery,
            NumberOfRepetitions = numberOfRepetitions
        };

        return await taskService.AddRecurringTaskAsync(task, repeatMode, repeatEvery, numberOfRepetitions);
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetDescription([Service] ITaskService taskService,
        IResolverContext context, [GlobalState] Guid currentUserId, Guid? taskId, Guid? groupId, string description)
    {
        if (taskId is not null)
        {
            return await taskService.SetTaskDescriptionAsync(taskId.Value, description, currentUserId) is null
                ? null
                : new[] { taskId.Value };
        }

        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskDescriptionAsync(groupId.Value, description, currentUserId);
        }

        throw new GraphQLException(ErrorBuilder
            .New()
            .SetMessage(ValidationErrors.TaskIdOrGroupIdIsRequired.Message)
            .SetCode(ValidationErrors.TaskIdOrGroupIdIsRequired.Code)
            .SetPath(context.Path)
            .Build());
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetDetails([Service] ITaskService taskService,
        IResolverContext context, [GlobalState] Guid currentUserId, Guid? taskId, Guid? groupId, string? details)
    {
        if (taskId is not null)
        {
            return await taskService.SetTaskDetailsAsync(taskId.Value, details, currentUserId) is null
                ? null
                : new[] { taskId.Value };
        }

        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskDetailsAsync(groupId.Value, details, currentUserId);
        }

        throw new GraphQLException(ErrorBuilder
            .New()
            .SetMessage(ValidationErrors.TaskIdOrGroupIdIsRequired.Message)
            .SetCode(ValidationErrors.TaskIdOrGroupIdIsRequired.Code)
            .SetPath(context.Path)
            .Build());
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetDirection([Service] ITaskService taskService,
        IResolverContext context, [GlobalState] Guid currentUserId, Guid? taskId, Guid? groupId, Guid? directionId)
    {
        if (taskId is not null)
        {
            return await taskService.SetTaskDirectionAsync(taskId.Value, directionId, currentUserId) is null
                ? null
                : new[] { taskId.Value };
        }

        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskDirectionAsync(groupId.Value, directionId, currentUserId);
        }

        throw new GraphQLException(ErrorBuilder
            .New()
            .SetMessage(ValidationErrors.TaskIdOrGroupIdIsRequired.Message)
            .SetCode(ValidationErrors.TaskIdOrGroupIdIsRequired.Code)
            .SetPath(context.Path)
            .Build());
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetDeletedAt([Service] ITaskService taskService,
        IResolverContext context, [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId,
        Guid? taskId, Guid? groupId, string? deletedAt)
    {
        DateTime? deletedAtUtc = deletedAt is null ? null : timeConverter.StringToDateTimeUtc(deletedAt);

        if (taskId is not null)
        {
            return await taskService.SetTaskDeletedAtAsync(taskId.Value, deletedAtUtc, currentUserId) is null
                ? null
                : new[] { taskId.Value };
        }

        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskDeletedAtAsync(groupId.Value, deletedAtUtc, currentUserId);
        }

        throw new GraphQLException(ErrorBuilder
            .New()
            .SetMessage(ValidationErrors.TaskIdOrGroupIdIsRequired.Message)
            .SetCode(ValidationErrors.TaskIdOrGroupIdIsRequired.Code)
            .SetPath(context.Path)
            .Build());
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetAssignee([Service] ITaskService taskService,
        IResolverContext context, [GlobalState] Guid currentUserId, Guid? taskId, Guid? groupId, Guid? assigneeId)
    {
        if (taskId is not null)
        {
            return await taskService.SetTaskAssigneeAsync(taskId.Value, assigneeId, currentUserId) is null
                ? null
                : new[] { taskId.Value };
        }

        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskAssigneeAsync(groupId.Value, assigneeId, currentUserId);
        }

        throw new GraphQLException(ErrorBuilder
            .New()
            .SetMessage(ValidationErrors.TaskIdOrGroupIdIsRequired.Message)
            .SetCode(ValidationErrors.TaskIdOrGroupIdIsRequired.Code)
            .SetPath(context.Path)
            .Build());
    }

    [Authorize]
    public async Task<IEnumerable<Guid>?> TaskSetStartAt([Service] ITaskService taskService,
        IResolverContext context, [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId,
        Guid? taskId, Guid? groupId,
        string startAt)
    {
        var startAtUtc = timeConverter.StringToDateTimeUtc(startAt);

        if (taskId is not null)
        {
            return await taskService.SetTaskStartAtAsync(taskId.Value, startAtUtc, currentUserId) is null
                ? null
                : new[] { taskId.Value };
        }

        if (groupId is not null)
        {
            return await taskService.SetRecurringTaskStartAtAsync(groupId.Value, startAtUtc, currentUserId);
        }

        throw new GraphQLException(ErrorBuilder
            .New()
            .SetMessage(ValidationErrors.TaskIdOrGroupIdIsRequired.Message)
            .SetCode(ValidationErrors.TaskIdOrGroupIdIsRequired.Code)
            .SetPath(context.Path)
            .Build());
    }

    [Authorize]
    public async Task<Guid?> TaskSetCompletedAt([Service] ITaskService taskService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid taskId, string? completedAt)
    {
        DateTime? completedAtUtc = completedAt is null ? null : timeConverter.StringToDateTimeUtc(completedAt);

        return await taskService.SetTaskCompletedAtAsync(taskId, completedAtUtc, currentUserId);
    }
}