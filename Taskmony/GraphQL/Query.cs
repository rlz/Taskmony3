using HotChocolate.Authorization;
using HotChocolate.Resolvers;
using Taskmony.Models;
using Taskmony.Services.Abstract;

namespace Taskmony.GraphQL;

[Authorize]
public class Query
{
    public async Task<IEnumerable<User>?> GetUsers([Service] IUserService userService,
        [GlobalState] Guid currentUserId, IResolverContext resolverContext,
        Guid[]? id, string[]? email, string[]? login, int? offset, int? limit)
    {
        return await userService.GetUsersAsync(id, email, login, offset, limit, currentUserId);
    }

    public async Task<IEnumerable<Models.Task>?> GetTasks([Service] ITaskService taskService,
        [GlobalState] Guid currentUserId, Guid[]? id, Guid?[]? directionId, int? offset, int? limit)
    {
        return await taskService.GetTasksAsync(id, directionId, offset, limit, currentUserId);
    }

    public async Task<IEnumerable<Idea>?> GetIdeas([Service] IIdeaService ideaService,
    [GlobalState] Guid currentUserId, Guid[]? id, Guid?[]? directionId, int? offset, int? limit)
    {
        return await ideaService.GetIdeasAsync(id, directionId, offset, limit, currentUserId);
    }

    public async Task<IEnumerable<Direction>?> GetDirections([Service] IDirectionService directionService,
        [GlobalState] Guid currentUserId, Guid[]? id, int? offset, int? limit)
    {
        return await directionService.GetDirectionsAsync(id, offset, limit, currentUserId);
    }
}