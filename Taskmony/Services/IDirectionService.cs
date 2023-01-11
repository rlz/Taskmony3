using Taskmony.Models;

namespace Taskmony.Services;

public interface IDirectionService
{
    Task<IEnumerable<Guid>> GetUserDirectionIds(Guid userId);

    Task<Direction?> GetDirectionByIdAsync(Guid id);

    Task<IEnumerable<Direction>> GetDirectionsAsync(Guid[]? id, int? offset, int? limit, Guid currentUserId);

    Task<IEnumerable<User>> GetDirectionMembersAsync(Guid id, Guid currentUserId);
}