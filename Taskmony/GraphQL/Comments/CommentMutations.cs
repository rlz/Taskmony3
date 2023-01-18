using HotChocolate.AspNetCore.Authorization;
using Taskmony.Models.Comments;
using Taskmony.Services;

namespace Taskmony.GraphQL.Comments;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class CommentMutations
{
    [Authorize]
    public async Task<Comment?> TaskAddComment([Service] ICommentService commentService, 
        [GlobalState] Guid userId, Guid taskId, string text)
    {
        var comment = new TaskComment
        {
            CreatedById = userId,
            Text = text,
            TaskId = taskId
        };

        return await commentService.AddComment(comment);
    }

    [Authorize]
    public async Task<Comment?> IdeaAddComment([Service] ICommentService commentService, 
        [GlobalState] Guid userId, Guid ideaId, string text)
    {
        var comment = new IdeaComment
        {
            CreatedById = userId,
            Text = text,
            IdeaId = ideaId
        };

        return await commentService.AddComment(comment);
    }

    [Authorize]
    public async Task<Guid?> CommentSetText([Service] ICommentService commentService,
        [GlobalState] Guid userId, Guid commentId, string text)
    {
        if (await commentService.SetCommentText(commentId, text, userId))
        {
            return commentId;
        }

        return null;
    }
}