using Taskmony.Models;
using Taskmony.Models.Comments;
using Taskmony.Models.Enums;

namespace Taskmony.GraphQL.Ideas;

public class IdeaType : ObjectType<Idea>
{
    protected override void Configure(IObjectTypeDescriptor<Idea> descriptor)
    {
        descriptor.Field(i => i.CreatedById).Ignore();
        descriptor.Field(i => i.DirectionId).Ignore();
        descriptor.Field(i => i.Subscriptions).Ignore();

        descriptor.Field(i => i.CreatedBy).Type<ObjectType<User>>();
        descriptor.Field(i => i.ReviewedAt).Type<StringType>();
        descriptor.Field(i => i.Description).Type<StringType>();
        descriptor.Field(i => i.CreatedAt).Type<StringType>();
        descriptor.Field(i => i.DeletedAt).Type<StringType>();
        descriptor.Field(i => i.Generation).Type<EnumType<Generation>>();
        descriptor.Field(i => i.Comments).Type<ListType<NonNullType<ObjectType<Comment>>>>();
        descriptor.Field(i => i.Notifications).Type<ListType<NonNullType<ObjectType<Notification>>>>();
    }
}