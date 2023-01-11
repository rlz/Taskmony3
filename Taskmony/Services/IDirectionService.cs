using Taskmony.Models;

namespace Taskmony.Services;

public interface IDirectionService
{
    Task<IEnumerable<Guid>> GetUserDirectionIds(Guid userId);

    Task<Direction?> GetDirectionByIdAsync(Guid id);
}