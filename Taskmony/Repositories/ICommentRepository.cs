using Taskmony.Models.Comments;

namespace Taskmony.Repositories;

public interface ICommentRepository
{
    Task<IEnumerable<Comment>> GetCommentsByTaskIdsAsync(Guid[] ids, int? offset, int? limit);

    Task<IEnumerable<Comment>> GetCommentsByIdeaIdsAsync(Guid[] ids, int? offset, int? limit);
    
    Task<IEnumerable<Comment>> GetCommentsByIdsAsync(Guid[] ids);

    Task<Comment?> GetCommentById(Guid id);

    Task AddComment(TaskComment comment);

    Task AddComment(IdeaComment comment);

    Task<bool> SaveChangesAsync();
}