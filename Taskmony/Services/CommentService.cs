using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Comments;
using Taskmony.Repositories.Abstract;
using Taskmony.Services.Abstract;
using Taskmony.ValueObjects;

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

    public async Task<IEnumerable<Comment>> GetCommentsByTaskIds(Guid[] ids, int? offset, int? limit)
    {
        int? limitValue = limit is null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset is null ? null : Offset.From(offset.Value).Value;

        return await _commentRepository.GetCommentsByTaskIdsAsync(ids, offsetValue, limitValue);
    }

    public async Task<IEnumerable<Comment>> GetCommentsByIdeaIds(Guid[] ids, int? offset, int? limit)
    {
        int? limitValue = limit is null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset is null ? null : Offset.From(offset.Value).Value;

        return await _commentRepository.GetCommentsByIdeaIdsAsync(ids, offsetValue, limitValue);
    }

    public async Task<IEnumerable<Comment>> GetCommentsByIdsAsync(Guid[] ids)
    {
        return await _commentRepository.GetCommentsByIdsAsync(ids);
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
        var commentText = CommentText.From(text);

        var comment = await GetCommentOrThrow(commentId, currentUserId);

        comment.Text = commentText;

        return await _commentRepository.SaveChangesAsync();
    }

    public async Task<bool> SetCommentDeletedAt(Guid commentId, DateTime? deletedAt, Guid currentUserId)
    {
        DeletedAt? commentDeletedAt = deletedAt is null ? null : DeletedAt.From(deletedAt.Value);

        var comment = await GetCommentOrThrow(commentId, currentUserId);

        if (deletedAt is not null && comment.DeletedAt is not null)
        {
            throw new DomainException(CommentErrors.AlreadyDeleted);
        }

        comment.DeletedAt = commentDeletedAt;

        return await _commentRepository.SaveChangesAsync();
    }

    private async Task<Comment> GetCommentOrThrow(Guid id, Guid currentUserId)
    {
        var comment = await _commentRepository.GetCommentById(id);

        if (comment is null)
        {
            throw new DomainException(CommentErrors.NotFound);
        }

        if (comment is IdeaComment ideaComment)
        {
            await _ideaService.GetIdeaOrThrowAsync(ideaComment.IdeaId, currentUserId);
        }
        else if (comment is TaskComment taskComment)
        {
            await _taskService.GetTaskOrThrowAsync(taskComment.TaskId, currentUserId);
        }

        return comment;
    }
}