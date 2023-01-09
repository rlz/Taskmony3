using Taskmony.Models;
using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;
using Taskmony.Services;

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

        descriptor.Field(n => n.ActionItem)
            .ResolveWith<Resolvers>(r => r.GetActionItem(default!, default!, default!))
            .Type<ActionItem>();
    }

    private class Resolvers
    {
        public async Task<IActionItem?> GetActionItem([Parent] Notification notification,
            [Service] INotificationService notificationService, [GlobalState] Guid userId)
        {
            if (notification.ActionItemType is null || notification.ActionItemId is null)
            {
                return null;
            }

            return await notificationService.GetActionItemAsync(notification.ActionItemType.Value,
                notification.ActionItemId.Value, userId);
        }
    }
}