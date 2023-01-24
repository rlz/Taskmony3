using Taskmony.Models;

namespace Taskmony.Services;

public interface IDirectionService
{
    Task<IEnumerable<Guid>> GetUserDirectionIds(Guid userId);
    
    Task<IEnumerable<Direction>> GetDirectionsAsync(Guid[]? id, int? offset, int? limit, Guid currentUserId);

    Task<IEnumerable<Direction>> GetDirectionsByIdsAsync(Guid[] ids);
    
    Task<ILookup<Guid, Guid>> GetMemberIdsAsync(Guid[] directionIds);
    
    Task<bool> AnyMemberWithId(Guid directionId, Guid memberId);

    Task<Direction> AddDirection(Direction direction);

    Task<bool> SetDirectionName(Guid id, string name, Guid currentUserId);

    Task<bool> SetDirectionDetails(Guid id, string? details, Guid currentUserId);

    Task<bool> AddMember(Guid directionId, Guid memberId, Guid currentUserId);

    Task<bool> RemoveMember(Guid directionId, Guid memberId, Guid currentUserId);

    Task<bool> SetDirectionDeletedAt(Guid id, DateTime? deletedAtUtc, Guid currentUserId);
}