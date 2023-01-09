using Taskmony.Models;
using Taskmony.Models.Comments;
using Taskmony.Services;

namespace Taskmony.GraphQL.Comments;

public class CommentType : ObjectType<Comment>
{
    protected override void Configure(IObjectTypeDescriptor<Comment> descriptor)
    {
        descriptor.Field(c => c.CreatedById).Ignore();
        descriptor.Field(c => c.ActionItemType).Ignore();

        descriptor.Field(c => c.CreatedBy)
            .Type<ObjectType<User>>()
            .ResolveWith<Resolvers>(r => r.GetCreatedBy(default!, default!, default!));

        descriptor.Field(c => c.Text).Type<StringType>();
        descriptor.Field(c => c.CreatedAt).Type<StringType>();
    }

    private class Resolvers
    {
        public async Task<User> GetCreatedBy([Parent] Comment comment, [Service] IUserService userService,
            [GlobalState] Guid userId)
        {
            var result = await userService.GetUsersAsync(new[] { comment.CreatedById },
                null, null, null, null, userId);
            return result.First();
        }
    }
}