using HotChocolate.AspNetCore.Authorization;
using Taskmony.Models.Comments;
using Taskmony.Services;
using Taskmony.ValueObjects;

namespace Taskmony.GraphQL.Comments;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class CommentMutations
{
    [Authorize]
    public async Task<Comment?> TaskAddComment([Service] ICommentService commentService,
        [GlobalState] Guid currentUserId, Guid taskId, string text)
    {
        var comment = new TaskComment
        {
            CreatedById = currentUserId,
            Text = CommentText.From(text),
            TaskId = taskId
        };

        return await commentService.AddComment(comment);
    }

    [Authorize]
    public async Task<Comment?> IdeaAddComment([Service] ICommentService commentService,
        [GlobalState] Guid currentUserId, Guid ideaId, string text)
    {
        var comment = new IdeaComment
        {
            CreatedById = currentUserId,
            Text = CommentText.From(text),
            IdeaId = ideaId
        };

        return await commentService.AddComment(comment);
    }

    [Authorize]
    public async Task<Guid?> CommentSetText([Service] ICommentService commentService,
        [GlobalState] Guid currentUserId, Guid commentId, string text)
    {
        if (await commentService.SetCommentText(commentId, text, currentUserId))
        {
            return commentId;
        }

        return null;
    }
}