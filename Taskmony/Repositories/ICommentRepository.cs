using Taskmony.Models.Comments;

namespace Taskmony.Repositories;

public interface ICommentRepository
{
    Task<IEnumerable<Comment>> GetTaskCommentsAsync(Guid taskId, int? offset, int? limit);
    
    Task<IEnumerable<Comment>> GetIdeaCommentsAsync(Guid ideaId, int? offset, int? limit);

    Task<Comment?> GetCommentById(Guid id);
    
    Task<bool> SaveChangesAsync();
}