using Taskmony.Models.Comments;

namespace Taskmony.Services.Abstract;

public interface ICommentService
{
    Task<IEnumerable<Comment>> GetCommentsByTaskIds(Guid[] ids, int? offset, int? limit);

    Task<IEnumerable<Comment>> GetCommentsByIdeaIds(Guid[] ids, int? offset, int? limit);
    
    Task<IEnumerable<Comment>> GetCommentsByIdsAsync(Guid[] ids);
    
    Task<Comment> AddComment(TaskComment comment);

    Task<Comment> AddComment(IdeaComment comment);

    Task<bool> SetCommentText(Guid id, string text, Guid currentUserId);

    Task<bool> SetCommentDeletedAt(Guid id, DateTime? deletedAt, Guid currentUserId);
}