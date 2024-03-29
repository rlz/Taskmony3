using Taskmony.Models.Directions;
using Taskmony.Services.Abstract;

namespace Taskmony.GraphQL.Directions;

[ExtendObjectType(typeof(Mutation))]
public class DirectionMutations
{
    public async Task<Direction?> DirectionAdd([Service] IDirectionService directionService,
        [GlobalState] Guid currentUserId, string name, string? details)
    {
        return await directionService.AddDirectionAsync(name, details, currentUserId);
    }

    public async Task<Guid?> DirectionSetName([Service] IDirectionService directionService,
        [GlobalState] Guid currentUserId, Guid directionId, string name)
    {
        return await directionService.SetDirectionNameAsync(directionId, name, currentUserId);
    }

    public async Task<Guid?> DirectionSetDetails([Service] IDirectionService directionService,
        [GlobalState] Guid currentUserId, Guid directionId, string? details)
    {
        return await directionService.SetDirectionDetailsAsync(directionId, details, currentUserId);
    }

    public async Task<Guid?> DirectionAddMember([Service] IDirectionService directionService,
        [GlobalState] Guid currentUserId, Guid directionId, Guid userId)
    {
        return await directionService.AddMemberAsync(directionId, userId, currentUserId);
    }

    public async Task<Guid?> DirectionRemoveMember([Service] IDirectionService directionService,
        [GlobalState] Guid currentUserId, Guid directionId, Guid userId)
    {
        return await directionService.RemoveMemberAsync(directionId, userId, currentUserId);
    }

    public async Task<Guid?> DirectionSetDeletedAt([Service] IDirectionService directionService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid directionId, string? deletedAt)
    {
        DateTime? deletedAtUtc = deletedAt is null ? null : timeConverter.StringToDateTimeUtc(deletedAt);

        return await directionService.SetDirectionDeletedAtAsync(directionId, deletedAtUtc, currentUserId);
    }
}