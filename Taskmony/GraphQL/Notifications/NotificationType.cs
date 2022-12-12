using Taskmony.Models;
using Taskmony.Models.Enums;

namespace Taskmony.GraphQL.Notifications;

public class NotificationType : ObjectType<Notification>
{
    protected override void Configure(IObjectTypeDescriptor<Notification> descriptor)
    {
        descriptor.Field(n => n.ActorId).Ignore();
        descriptor.Field(n => n.NotifiableId).Ignore();
        descriptor.Field(n => n.NotifiableType).Ignore();
        descriptor.Field(n => n.ActionItemId).Ignore();
        descriptor.Field(n => n.ActionItemType).Ignore();

        descriptor.Field(n => n.Actor).Type<ObjectType<User>>();
        descriptor.Field(n => n.ModifiedAt).Type<StringType>();
        descriptor.Field(n => n.ActionType).Type<EnumType<ActionType>>();

        descriptor.Field("actionItem")
            .ResolveWith<Resolvers>(r => r.GetActionItem(default!))
            .Type<ActionItem>();
    }

    private class Resolvers
    {
        public object GetActionItem(Notification notification)
        {
            // TODO: resolve action item
            return new User();
        }
    }
}