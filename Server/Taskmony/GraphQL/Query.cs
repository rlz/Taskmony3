using HotChocolate.Authorization;
using Taskmony.Models.Directions;
using Taskmony.Models.Ideas;
using Taskmony.Models.Users;
using Taskmony.Services.Abstract;
using Task = Taskmony.Models.Tasks.Task;

namespace Taskmony.GraphQL;

[Authorize]
public class Query
{
    public async Task<IEnumerable<User>?> GetUsers([Service] IUserService userService,
        [GlobalState] Guid currentUserId, Guid[]? id, string[]? email, string[]? login, int? offset, int? limit)
    {
        return await userService.GetUsersAsync(
            id: id,
            email: email,
            login: login,
            offset: offset,
            limit: limit,
            currentUserId: currentUserId);
    }

    public async Task<IEnumerable<Task>?> GetTasks([Service] ITaskService taskService,
        [GlobalState] Guid currentUserId, [Service] ITimeConverter timeConverter, Guid[]? id, Guid?[]? directionId,
        bool? completed, bool? deleted, string? lastCompletedAt, string? lastDeletedAt, int? offset, int? limit)
    {
        DateTime? lastCompletedAtUtc =
            lastCompletedAt == null ? null : timeConverter.StringToDateTimeUtc(lastCompletedAt);
        DateTime? lastDeletedAtUtc = lastDeletedAt == null ? null : timeConverter.StringToDateTimeUtc(lastDeletedAt);

        return await taskService.GetTasksAsync(
            id: id,
            directionId: directionId,
            completed: completed,
            deleted: deleted,
            lastCompletedAt: lastCompletedAtUtc,
            lastDeletedAt: lastDeletedAtUtc,
            offset: offset,
            limit: limit,
            currentUserId: currentUserId);
    }

    public async Task<IEnumerable<Idea>?> GetIdeas([Service] IIdeaService ideaService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid[]? id, Guid?[]? directionId,
        bool? deleted, string? lastDeletedAt, int? offset, int? limit)
    {
        DateTime? lastDeletedAtUtc = lastDeletedAt == null ? null : timeConverter.StringToDateTimeUtc(lastDeletedAt);

        return await ideaService.GetIdeasAsync(
            id: id,
            directionId: directionId,
            deleted: deleted,
            lastDeletedAt: lastDeletedAtUtc,
            offset: offset,
            limit: limit,
            currentUserId: currentUserId);
    }

    public async Task<IEnumerable<Direction>?> GetDirections([Service] IDirectionService directionService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid[]? id, bool? deleted,
        string? lastDeletedAt, int? offset, int? limit)
    {
        DateTime? lastDeletedAtUtc = lastDeletedAt == null ? null : timeConverter.StringToDateTimeUtc(lastDeletedAt);

        return await directionService.GetDirectionsAsync(
            id: id,
            deleted: deleted,
            lastDeletedAt: lastDeletedAtUtc,
            offset: offset,
            limit: limit,
            currentUserId: currentUserId);
    }
}