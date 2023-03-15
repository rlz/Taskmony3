using Taskmony.GraphQL.Comments;
using Taskmony.GraphQL.Ideas;
using Taskmony.GraphQL.Tasks;
using Taskmony.GraphQL.Users;

namespace Taskmony.GraphQL.Notifications;

public class ActionItem : UnionType
{
    protected override void Configure(IUnionTypeDescriptor descriptor)
    {
        // The object types that belong to this union
        descriptor.Type<UserType>();
        descriptor.Type<TaskType>();
        descriptor.Type<IdeaType>();
        descriptor.Type<CommentType>();
    }
}