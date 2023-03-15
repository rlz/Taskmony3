using Taskmony.Models;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Repositories.Abstract;

public interface IIdeaRepository
{
    Task<IEnumerable<Idea>> GetAsync(Guid[]? id, Guid?[] directionId,
        int? offset, int? limit, Guid userId);
    
    Task<IEnumerable<Idea>> GetByIdsAsync(IEnumerable<Guid> ids);

    Task<Idea?> GetByIdAsync(Guid id);

    Task AddAsync(Idea idea);

    Task<bool> SaveChangesAsync();
}