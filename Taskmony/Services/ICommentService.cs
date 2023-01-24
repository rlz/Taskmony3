using Taskmony.Models.Comments;

namespace Taskmony.Services;

public interface ICommentService
{
    Task<IEnumerable<Comment>> GetCommentsByTaskIds(Guid[] ids, int? offset, int? limit);

    Task<IEnumerable<Comment>> GetCommentsByIdeaIds(Guid[] ids, int? offset, int? limit);
    
    Task<IEnumerable<Comment>> GetCommentsByIdsAsync(Guid[] ids);
    
    Task<Comment> AddComment(TaskComment comment);

    Task<Comment> AddComment(IdeaComment comment);

    Task<bool> SetCommentText(Guid commentId, string text, Guid currentUserId);
}