using Taskmony.Models.Comments;

namespace Taskmony.Services;

public interface ICommentService
{
    Task<IEnumerable<Comment>> GetTaskCommentsAsync(Guid taskId, int? offset, int? limit);

    Task<IEnumerable<Comment>> GetIdeaCommentsAsync(Guid ideaId, int? offset, int? limit);
}