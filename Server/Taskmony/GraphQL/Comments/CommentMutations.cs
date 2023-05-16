using Taskmony.Models.Comments;
using Taskmony.Services.Abstract;

namespace Taskmony.GraphQL.Comments;

[ExtendObjectType(typeof(Mutation))]
public class CommentMutations
{
    public async Task<Comment?> TaskAddComment([Service] ICommentService commentService,
        [GlobalState] Guid currentUserId, Guid taskId, string text)
    {
        return await commentService.AddTaskComment(taskId, text, currentUserId);
    }

    public async Task<Comment?> IdeaAddComment([Service] ICommentService commentService,
        [GlobalState] Guid currentUserId, Guid ideaId, string text)
    {
        return await commentService.AddIdeaComment(ideaId, text, currentUserId);
    }

    public async Task<Guid?> CommentSetText([Service] ICommentService commentService,
        [GlobalState] Guid currentUserId, Guid commentId, string text)
    {
        return await commentService.SetCommentText(commentId, text, currentUserId);
    }

    public async Task<Guid?> CommentSetDeletedAt([Service] ICommentService commentService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid commentId, string? deletedAt)
    {
        DateTime? deletedAtUtc = deletedAt is null ? null : timeConverter.StringToDateTimeUtc(deletedAt);

        return await commentService.SetCommentDeletedAt(commentId, deletedAtUtc, currentUserId);
    }
}