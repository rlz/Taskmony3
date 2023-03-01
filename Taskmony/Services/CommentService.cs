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
    private readonly INotificationService _notificationService;

    public CommentService(ICommentRepository commentRepository, ITaskService taskService,
        IIdeaService ideaService, INotificationService notificationService)
    {
        _commentRepository = commentRepository;
        _taskService = taskService;
        _ideaService = ideaService;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<Comment>> GetCommentsByTaskIds(Guid[] ids, int? offset, int? limit)
    {
        int? limitValue = limit is null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset is null ? null : Offset.From(offset.Value).Value;

        return await _commentRepository.GetByTaskIdsAsync(ids, offsetValue, limitValue);
    }

    public async Task<IEnumerable<Comment>> GetCommentsByIdeaIds(Guid[] ids, int? offset, int? limit)
    {
        int? limitValue = limit is null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset is null ? null : Offset.From(offset.Value).Value;

        return await _commentRepository.GetByIdeaIdsAsync(ids, offsetValue, limitValue);
    }

    public async Task<IEnumerable<Comment>> GetCommentsByIdsAsync(Guid[] ids)
    {
        return await _commentRepository.GetByIdsAsync(ids);
    }

    public async Task<Comment?> AddComment(TaskComment comment)
    {
        var task = await _taskService.GetTaskOrThrowAsync(comment.TaskId, comment.CreatedById);

        await _commentRepository.AddAsync(comment);

        if (!await _commentRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyCommentAddedAsync(task, comment.Id, comment.CreatedById, comment.CreatedAt);

        return comment;
    }

    public async Task<Comment?> AddComment(IdeaComment comment)
    {
        var idea = await _ideaService.GetIdeaOrThrowAsync(comment.IdeaId, comment.CreatedById);
        
        await _commentRepository.AddAsync(comment);

        if (!await _commentRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyCommentAddedAsync(idea, comment.Id, comment.CreatedById, comment.CreatedAt);

        return comment;
    }

    public async Task<Guid?> SetCommentText(Guid commentId, string text, Guid currentUserId)
    {
        var commentText = CommentText.From(text);

        var comment = await GetCommentOrThrow(commentId, currentUserId);

        comment.Text = commentText;

        return await _commentRepository.SaveChangesAsync() ? commentId : null;
    }

    public async Task<Guid?> SetCommentDeletedAt(Guid commentId, DateTime? deletedAt, Guid currentUserId)
    {
        var commentDeletedAt = deletedAt is null ? null : DeletedAt.From(deletedAt.Value);

        var comment = await GetCommentOrThrow(commentId, currentUserId);

        if (deletedAt is not null && comment.DeletedAt is not null)
        {
            throw new DomainException(CommentErrors.AlreadyDeleted);
        }

        comment.DeletedAt = commentDeletedAt;

        return await _commentRepository.SaveChangesAsync() ? commentId : null;
    }

    private async Task<Comment> GetCommentOrThrow(Guid id, Guid currentUserId)
    {
        var comment = await _commentRepository.GetByIdAsync(id);

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