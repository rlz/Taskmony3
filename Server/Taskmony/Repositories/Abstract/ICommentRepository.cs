using Taskmony.Models.Comments;

namespace Taskmony.Repositories.Abstract;

public interface ICommentRepository
{
    Task<IEnumerable<Comment>> GetByTaskIdsAsync(IEnumerable<Guid> ids, int? offset, int? limit);

    Task<IEnumerable<Comment>> GetByIdeaIdsAsync(IEnumerable<Guid> ids, int? offset, int? limit);
    
    Task<IEnumerable<Comment>> GetByIdsAsync(IEnumerable<Guid> ids);

    Task<Comment?> GetByIdAsync(Guid id);

    Task AddAsync(Comment comment);

    Task<bool> SaveChangesAsync();
}