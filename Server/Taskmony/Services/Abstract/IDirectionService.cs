using Taskmony.Models.Directions;

namespace Taskmony.Services.Abstract;

public interface IDirectionService
{
    Task<IEnumerable<Direction>> GetDirectionsAsync(Guid[]? id, int? offset, int? limit, Guid currentUserId);

    Task<IEnumerable<Direction>> GetDirectionsByIdsAsync(Guid[] ids);

    Task<ILookup<Guid, Guid>> GetMemberIdsAsync(Guid[] directionIds, int? offset, int? limit);

    Task<Direction> AddDirectionAsync(string name, string? details, Guid currentUserId);

    Task<Guid?> SetDirectionNameAsync(Guid id, string name, Guid currentUserId);

    Task<Guid?> SetDirectionDetailsAsync(Guid id, string? details, Guid currentUserId);

    Task<Guid?> AddMemberAsync(Guid directionId, Guid memberId, Guid currentUserId);

    Task<Guid?> RemoveMemberAsync(Guid directionId, Guid memberId, Guid currentUserId);

    Task<Guid?> SetDirectionDeletedAtAsync(Guid id, DateTime? deletedAtUtc, Guid currentUserId);
}