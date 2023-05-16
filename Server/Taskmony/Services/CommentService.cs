using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Comments;
using Taskmony.Models.Directions;
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

    public async Task<IEnumerable<Comment>> GetCommentsByTaskIds(IEnumerable<Guid> ids, int? offset, int? limit)
    {
        int? limitValue = limit == null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset == null ? null : Offset.From(offset.Value).Value;

        return await _commentRepository.GetByTaskIdsAsync(ids, offsetValue, limitValue);
    }

    public async Task<IEnumerable<Comment>> GetCommentsByIdeaIds(IEnumerable<Guid> ids, int? offset, int? limit)
    {
        int? limitValue = limit == null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset == null ? null : Offset.From(offset.Value).Value;

        return await _commentRepository.GetByIdeaIdsAsync(ids, offsetValue, limitValue);
    }

    public async Task<IEnumerable<Comment>> GetCommentsByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _commentRepository.GetByIdsAsync(ids);
    }

    public async Task<Comment?> AddTaskComment(Guid taskId, string text, Guid currentUserId)
    {
        var comment = new TaskComment(CommentText.From(text), currentUserId, taskId);

        var task = await _taskService.GetTaskOrThrowAsync(comment.TaskId, comment.CreatedById);

        return await AddComment(comment, task);
    }

    public async Task<Comment?> AddIdeaComment(Guid ideaId, string text, Guid currentUserId)
    {
        var comment = new IdeaComment(CommentText.From(text), currentUserId, ideaId);

        var idea = await _ideaService.GetIdeaOrThrowAsync(comment.IdeaId, comment.CreatedById);

        return await AddComment(comment, idea);
    }

    private async Task<Comment?> AddComment(Comment comment, DirectionEntity entity)
    {
        await _commentRepository.AddAsync(comment);

        if (!await _commentRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyCommentAddedAsync(entity, comment.Id, comment.CreatedById, comment.CreatedAt);

        return comment;
    }

    public async Task<Guid?> SetCommentText(Guid commentId, string text, Guid currentUserId)
    {
        var commentText = CommentText.From(text);

        var comment = await GetCommentOrThrow(commentId, currentUserId);

        comment.UpdateText(commentText);

        return await _commentRepository.SaveChangesAsync() ? commentId : null;
    }

    public async Task<Guid?> SetCommentDeletedAt(Guid commentId, DateTime? deletedAt, Guid currentUserId)
    {
        var newDeletedAt = deletedAt == null ? null : DeletedAt.From(deletedAt.Value);

        var comment = await GetCommentOrThrow(commentId, currentUserId);

        comment.UpdateDeletedAt(newDeletedAt);

        return await _commentRepository.SaveChangesAsync() ? commentId : null;
    }

    private async Task<Comment> GetCommentOrThrow(Guid id, Guid currentUserId)
    {
        var comment = await _commentRepository.GetByIdAsync(id);

        if (comment == null)
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