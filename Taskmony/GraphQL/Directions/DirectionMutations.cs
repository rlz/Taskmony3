using HotChocolate.AspNetCore.Authorization;
using Taskmony.Models;
using Taskmony.Services;

namespace Taskmony.GraphQL.Directions;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class DirectionMutations
{
    [Authorize]
    public async Task<Direction?> DirectionAdd([Service] IDirectionService directionService,
        [GlobalState] Guid currentUserId, string name, string? details)
    {
        var direction = new Direction
        {
            CreatedById = currentUserId,
            Name = name,
            Details = details
        };

        return await directionService.AddDirection(direction);
    }

    [Authorize]
    public async Task<Guid?> DirectionSetName([Service] IDirectionService directionService,
        [GlobalState] Guid currentUserId, Guid directionId, string name)
    {
        if (await directionService.SetDirectionName(directionId, name, currentUserId))
        {
            return directionId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> DirectionSetDetails([Service] IDirectionService directionService,
        [GlobalState] Guid currentUserId, Guid directionId, string? details)
    {
        if (await directionService.SetDirectionDetails(directionId, details, currentUserId))
        {
            return directionId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> DirectionAddMember([Service] IDirectionService directionService,
        [GlobalState] Guid currentUserId, Guid directionId, Guid userId)
    {
        if (await directionService.AddMember(directionId, userId, currentUserId))
        {
            return directionId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> DirectionRemoveMember([Service] IDirectionService directionService,
        [GlobalState] Guid currentUserId, Guid directionId, Guid userId)
    {
        if (await directionService.RemoveMember(directionId, userId, currentUserId))
        {
            return directionId;
        }

        return null;
    }
}