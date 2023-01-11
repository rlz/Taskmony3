using Taskmony.Models;

namespace Taskmony.Repositories;

public interface IDirectionRepository
{
    Task<Direction?> GetDirectionByIdAsync(Guid id);

    Task<IEnumerable<Direction>> GetUserDirectionsAsync(Guid userId);
    
    Task<bool> SaveChangesAsync();
}