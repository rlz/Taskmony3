using Taskmony.Models.Comments;

namespace Taskmony.Services.Abstract;

public interface ICommentService
{
    Task<IEnumerable<Comment>> GetCommentsByTaskIds(IEnumerable<Guid> ids, int? offset, int? limit);

    Task<IEnumerable<Comment>> GetCommentsByIdeaIds(IEnumerable<Guid> ids, int? offset, int? limit);

    Task<IEnumerable<Comment>> GetCommentsByIdsAsync(IEnumerable<Guid> ids);

    Task<Comment?> AddTaskComment(Guid taskId, string text, Guid currentUserId);

    Task<Comment?> AddIdeaComment(Guid ideaId, string text, Guid currentUserId);

    Task<Guid?> SetCommentText(Guid id, string text, Guid currentUserId);

    Task<Guid?> SetCommentDeletedAt(Guid id, DateTime? deletedAt, Guid currentUserId);
}