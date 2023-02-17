using Taskmony.Models;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Repositories.Abstract;

public interface IDirectionRepository
{
    Task<Direction?> GetDirectionByIdAsync(Guid id);

    Task<IEnumerable<Direction>> GetDirectionsAsync(Guid[]? id, int? offset, int? limit, Guid userId);

    Task<IEnumerable<Guid>> GetUserDirectionIds(Guid userId);

    Task<IEnumerable<Direction>> GetDirectionByIdsAsync(Guid[] ids);

    Task<ILookup<Guid, Guid>> GetMemberIdsAsync(Guid[] directionIds, int? offset, int? limit);

    Task<bool> AnyMemberWithIdAsync(Guid directionId, Guid memberId);

    Task<IEnumerable<Guid>> GetIdsOfUsersWithCommonDirection(Guid user, IEnumerable<Guid> users);

    Task AddDirectionAsync(Direction direction);

    Task AddMemberAsync(Membership membership);

    void RemoveMember(Membership membership);

    Task<bool> SaveChangesAsync();
}