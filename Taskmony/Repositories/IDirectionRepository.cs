using Taskmony.Models;

namespace Taskmony.Repositories;

public interface IDirectionRepository
{
    Task<Direction?> GetDirectionAsync(Guid directionId);

    Task<IEnumerable<Direction>> GetUserDirectionsAsync(Guid userId);
    
    Task<bool> SaveChangesAsync();
}