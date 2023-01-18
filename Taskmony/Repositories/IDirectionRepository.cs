using Taskmony.Models;

namespace Taskmony.Repositories;

public interface IDirectionRepository
{
    Task<Direction?> GetDirectionByIdAsync(Guid id);

    Task<IEnumerable<Direction>> GetDirectionsAsync(Guid[]? id, int? offset, int? limit, Guid userId);

    Task<IEnumerable<Guid>> GetMemberIdsAsync(Guid directionId);

    Task<bool> AnyMemberWithId(Guid directionId, Guid memberId);

    Task<bool> SaveChangesAsync();
}