using Taskmony.Models.Comments;
using Taskmony.Repositories;

namespace Taskmony.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    
    public CommentService(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }
    
    public async Task<IEnumerable<Comment>> GetTaskCommentsAsync(Guid taskId, int? offset, int? limit)
    {
        return await _commentRepository.GetTaskCommentsAsync(taskId, offset, limit);
    }

    public async Task<IEnumerable<Comment>> GetIdeaCommentsAsync(Guid ideaId, int? offset, int? limit)
    {
        return await _commentRepository.GetIdeaCommentsAsync(ideaId, offset, limit);
    }
}