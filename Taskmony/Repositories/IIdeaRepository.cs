using Taskmony.Models;

namespace Taskmony.Repositories;

public interface IIdeaRepository
{
    Task<IEnumerable<Idea>> GetIdeasAsync(Guid[]? id, Guid?[] directionId,
        int? offset, int? limit, Guid userId);
        
    Task<bool> SaveChangesAsync();
}