using Taskmony.Models.Comments;

namespace Taskmony.Services;

public interface ICommentService
{
    Task<IEnumerable<Comment>> GetTaskCommentsAsync(Guid taskId, int? offset, int? limit);

    Task<IEnumerable<Comment>> GetIdeaCommentsAsync(Guid ideaId, int? offset, int? limit);

    Task<Comment?> GetCommentById(Guid id);

    Task<Comment> AddComment(TaskComment comment);

    Task<Comment> AddComment(IdeaComment comment);

    Task<bool> SetCommentText(Guid commentId, string text, Guid currentUserId);
}