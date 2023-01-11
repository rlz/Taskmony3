using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Resolvers;
using Taskmony.Errors;
using Taskmony.Models;
using Taskmony.Services;

namespace Taskmony.GraphQL;

public class Query
{
    [Authorize]
    public async Task<IEnumerable<User>?> GetUsers([Service] IUserService userService,
        [GlobalState] Guid userId,
        IResolverContext resolverContext,
        Guid[]? id, string[]? email, string[]? login, int? offset, int? limit)
    {
        if (id is not null && id.Any(x => x == Guid.Empty))
        {
            for (var i = 0; i < id.Length; i++)
            {
                if (id[i] == Guid.Empty)
                {
                    resolverContext.ReportError(
                        ErrorBuilder.New()
                            .SetMessage(ValidationErrors.InvalidId.Message)
                            .SetCode(ValidationErrors.InvalidId.Code)
                            .SetPath(resolverContext.Path.Append(i))
                            .Build());
                }
            }
        }

        return await userService.GetUsersAsync(id, email, login, offset, limit, userId);
    }

    [Authorize]
    public async Task<IEnumerable<Models.Task>?> GetTasks([Service] ITaskService taskService,
        [GlobalState] Guid userId, Guid[]? id, Guid?[]? directionId, int? offset, int? limit)
    {
        return await taskService.GetTasksAsync(id, directionId, offset, limit, userId);
    }

    [Authorize]
    public async Task<IEnumerable<Idea>?> GetIdeas([Service] IIdeaService ideaService,
    [GlobalState] Guid userId, Guid[]? id, Guid?[]? directionId, int? offset, int? limit)
    {
        return await ideaService.GetIdeasAsync(id, directionId, offset, limit, userId);
    }

    [Authorize]
    public async Task<IEnumerable<Direction>?> GetDirections([Service] IDirectionService directionService,
        [GlobalState] Guid userId, Guid[]? id, int? offset, int? limit)
    {
        return await directionService.GetDirectionsAsync(id, offset, limit, userId);
    }
}