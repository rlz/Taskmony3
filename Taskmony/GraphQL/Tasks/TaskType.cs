using Taskmony.Models;
using Taskmony.Models.Comments;
using Task = Taskmony.Models.Task;

namespace Taskmony.GraphQL.Tasks;

public class TaskType : ObjectType<Task>
{
    protected override void Configure(IObjectTypeDescriptor<Task> descriptor)
    {
        descriptor.Field(t => t.AssigneeId).Ignore();
        descriptor.Field(t => t.CreatedById).Ignore();
        descriptor.Field(t => t.DirectionId).Ignore();
        descriptor.Field(t => t.Subscriptions).Ignore();

        descriptor.Field(t => t.CreatedBy).Type<ObjectType<User>>();
        descriptor.Field(t => t.CompletedAt).Type<StringType>();
        descriptor.Field(t => t.Description).Type<StringType>();
        descriptor.Field(t => t.CreatedAt).Type<StringType>();
        descriptor.Field(t => t.DeletedAt).Type<StringType>();
        descriptor.Field(t => t.StartAt).Type<StringType>();
        descriptor.Field(t => t.Comments).Type<ListType<NonNullType<ObjectType<Comment>>>>();
        descriptor.Field(t => t.GroupId).Type<IdType>();
        descriptor.Field(i => i.Notifications).Type<ListType<NonNullType<ObjectType<Notification>>>>();
    }
}