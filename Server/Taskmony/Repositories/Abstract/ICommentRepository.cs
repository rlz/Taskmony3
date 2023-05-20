using Taskmony.Models.Comments;

namespace Taskmony.Repositories.Abstract;

public interface ICommentRepository
{
    Task<IEnumerable<Comment>> GetByTaskIdsAsync(IEnumerable<Guid> ids, int? offset, int? limit);

    Task<IEnumerable<Comment>> GetByIdeaIdsAsync(IEnumerable<Guid> ids, int? offset, int? limit);

    Task<IEnumerable<Comment>> GetByIdsAsync(IEnumerable<Guid> ids);

    Task<Comment?> GetByIdAsync(Guid id);

    Task AddAsync(Comment comment);

    Task SoftDeleteTaskCommentsAsync(IEnumerable<Guid> taskIds);

    Task SoftDeleteIdeaCommentsAsync(IEnumerable<Guid> ideaIds);

    Task UndeleteTaskCommentsAsync(IEnumerable<Guid> taskIds, DateTime deletedAt);

    Task UndeleteIdeaCommentsAsync(IEnumerable<Guid> ideaIds, DateTime deletedAt);

    Task HardDeleteSoftDeletedComments(DateTime deletedBeforeOrAt);

    Task<bool> SaveChangesAsync();
}