using Taskmony.Models;
using Taskmony.Models.Comments;

namespace Taskmony.GraphQL.Comments;

public class CommentType : ObjectType<Comment>
{
    protected override void Configure(IObjectTypeDescriptor<Comment> descriptor)
    {
        descriptor.Field(c => c.CreatedById).Ignore();

        descriptor.Field(n => n.CreatedBy).Type<ObjectType<User>>();
        descriptor.Field(c => c.Text).Type<StringType>();
        descriptor.Field(c => c.CreatedAt).Type<StringType>();
    }
}