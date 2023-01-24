using Taskmony.Models;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Repositories;

public interface IIdeaRepository
{
    Task<IEnumerable<Idea>> GetIdeasAsync(Guid[]? id, Guid?[] directionId,
        int? offset, int? limit, Guid userId);
    
    Task<IEnumerable<Idea>> GetIdeaByIdsAsync(Guid[] ids);

    Task<Idea?> GetIdeaByIdAsync(Guid id);

    Task AddIdeaAsync(Idea idea);

    Task<bool> SaveChangesAsync();
}