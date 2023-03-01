using Taskmony.Models;

namespace Taskmony.Services.Abstract;

public interface IDirectionService
{
    Task<IEnumerable<Guid>> GetUserDirectionIdsAsync(Guid userId);
    
    Task<IEnumerable<Direction>> GetDirectionsAsync(Guid[]? id, int? offset, int? limit, Guid currentUserId);

    Task<IEnumerable<Direction>> GetDirectionsByIdsAsync(Guid[] ids);
    
    Task<ILookup<Guid, Guid>> GetMemberIdsAsync(Guid[] directionIds, int? offset, int? limit);
    
    Task<bool> AnyMemberWithIdAsync(Guid directionId, Guid memberId);

    Task<Direction> AddDirectionAsync(Direction direction);

    Task<Guid?> SetDirectionName(Guid id, string name, Guid currentUserId);

    Task<Guid?> SetDirectionDetails(Guid id, string? details, Guid currentUserId);

    Task<Guid?> AddMemberAsync(Guid directionId, Guid memberId, Guid currentUserId);

    Task<Guid?> RemoveMemberAsync(Guid directionId, Guid memberId, Guid currentUserId);

    Task<Guid?> SetDirectionDeletedAtAsync(Guid id, DateTime? deletedAtUtc, Guid currentUserId);
}