using Taskmony.Models;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Repositories;

public interface IDirectionRepository
{
    Task<Direction?> GetDirectionByIdAsync(Guid id);

    Task<IEnumerable<Direction>> GetDirectionsAsync(Guid[]? id, int? offset, int? limit, Guid userId);

    Task<IEnumerable<Direction>> GetDirectionByIdsAsync(Guid[] ids);

    Task<IEnumerable<Guid>> GetMemberIdsAsync(Guid directionId);

    Task<bool> AnyMemberWithIdAsync(Guid directionId, Guid memberId);

    Task AddDirectionAsync(Direction direction);

    void AddMember(Membership membership);

    void RemoveMember(Membership membership);

    Task<bool> SaveChangesAsync();
}