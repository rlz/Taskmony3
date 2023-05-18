using Taskmony.Models.Directions;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Repositories.Abstract;

public interface IDirectionRepository
{
    Task<Direction?> GetByIdAsync(Guid id);

    Task<IEnumerable<Direction>> GetAsync(Guid[]? id, bool? deleted, DateTime? lastDeletedAt, int? offset, int? limit,
        Guid userId);

    Task<IEnumerable<Guid>> GetUserDirectionIdsAsync(Guid userId);

    Task<IEnumerable<Direction>> GetByIdsAsync(IEnumerable<Guid> ids);

    Task<ILookup<Guid, Guid>> GetMemberIdsAsync(Guid[] directionIds, int? offset, int? limit);

    Task<bool> AnyMemberWithIdAsync(Guid directionId, Guid memberId);

    Task<bool> AnyMemberInDirectionAsync(Guid directionId);

    Task<bool> AnyMemberOtherThanUserInDirectionAsync(Guid directionId, Guid userId);

    Task AddAsync(Direction direction);

    Task AddMemberAsync(Membership membership);

    void RemoveMember(Membership membership);

    void Delete(Direction direction);

    void HardDeleteSoftDeletedDirectionsWithChildren(DateTime deletedBeforeOrAt);

    Task<bool> SaveChangesAsync();
}