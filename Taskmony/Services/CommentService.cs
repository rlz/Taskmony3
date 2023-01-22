using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Comments;
using Taskmony.Repositories;

namespace Taskmony.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly ITaskService _taskService;
    private readonly IIdeaService _ideaService;

    public CommentService(ICommentRepository commentRepository, ITaskService taskService, IIdeaService ideaService)
    {
        _commentRepository = commentRepository;
        _taskService = taskService;
        _ideaService = ideaService;
    }

    public async Task<IEnumerable<Comment>> GetTaskCommentsAsync(Guid taskId, int? offset, int? limit)
    {
        return await _commentRepository.GetTaskCommentsAsync(taskId, offset, limit);
    }

    public async Task<IEnumerable<Comment>> GetIdeaCommentsAsync(Guid ideaId, int? offset, int? limit)
    {
        return await _commentRepository.GetIdeaCommentsAsync(ideaId, offset, limit);
    }

    public async Task<Comment?> GetCommentById(Guid id)
    {
        return await _commentRepository.GetCommentById(id);
    }

    public async Task<IEnumerable<Comment>> GetCommentsByTaskIds(Guid[] ids, int? offset, int? limit)
    {
        return await _commentRepository.GetCommentsByTaskIdsAsync(ids, offset, limit);
    }

    public async Task<IEnumerable<Comment>> GetCommentsByIdeaIds(Guid[] ids, int? offset, int? limit)
    {
        return await _commentRepository.GetCommentsByIdeaIdsAsync(ids, offset, limit);
    }

    public async Task<Comment> AddComment(TaskComment comment)
    {
        await _taskService.GetTaskOrThrowAsync(comment.TaskId, comment.CreatedById);

        await _commentRepository.AddComment(comment);

        await _commentRepository.SaveChangesAsync();

        return comment;
    }

    public async Task<Comment> AddComment(IdeaComment comment)
    {
        await _ideaService.GetIdeaOrThrowAsync(comment.IdeaId, comment.CreatedById);

        await _commentRepository.AddComment(comment);

        await _commentRepository.SaveChangesAsync();

        return comment;
    }

    public async Task<bool> SetCommentText(Guid commentId, string text, Guid currentUserId)
    {
        var comment = await GetCommentOrThrow(commentId, currentUserId);

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new DomainException(ValidationErrors.InvalidCommentText);
        }

        comment.Text = text;

        return await _commentRepository.SaveChangesAsync();
    }

    private async Task<Comment> GetCommentOrThrow(Guid id, Guid currentUserId)
    {
        var comment = await _commentRepository.GetCommentById(id);

        if (comment is null)
        {
            throw new DomainException(CommentErrors.NotFound);
        }

        if (comment is IdeaComment)
        {
            await _ideaService.GetIdeaOrThrowAsync(((IdeaComment)comment).IdeaId, currentUserId);
        }
        else if (comment is TaskComment)
        {
            await _taskService.GetTaskOrThrowAsync(((TaskComment)comment).TaskId, currentUserId);
        }

        return comment;
    }
}