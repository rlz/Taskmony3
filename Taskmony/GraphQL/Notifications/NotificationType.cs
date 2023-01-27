using Taskmony.GraphQL.DataLoaders;
using Taskmony.GraphQL.Users;
using Taskmony.Models;
using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;

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

        descriptor.Field(n => n.ModifiedAt).Type<StringType>();
        descriptor.Field(n => n.ActionType).Type<EnumType<ActionType>>();

        descriptor.Field(n => n.ActionItem)
            .ResolveWith<Resolvers>(r => r.GetActionItem(default!, default!, default!, default!, default!))
            .Type<ActionItem>();

        descriptor.Field(n => n.Actor)
            .Type<UserType>()
            .ResolveWith<Resolvers>(r => r.GetActor(default!, default!));
    }

    private class Resolvers
    {
        public async Task<IActionItem?> GetActionItem([Parent] Notification notification,
            UserByIdDataLoader userById, TaskByIdDataLoader taskById,
            IdeaByIdDataLoader ideaById, CommentByIdDataLoader commentById)
        {
            if (notification.ActionItemId is null)
            {
                return null;
            }

            return notification.ActionItemType switch
            {
                ActionItemType.User => await userById.LoadAsync(notification.ActionItemId.Value),
                ActionItemType.Task => await taskById.LoadAsync(notification.ActionItemId.Value),
                ActionItemType.Idea => await ideaById.LoadAsync(notification.ActionItemId.Value),
                ActionItemType.Comment => await commentById.LoadAsync(notification.ActionItemId.Value),
                _ => null
            };
        }

        public async Task<User> GetActor([Parent] Notification notification, UserByIdDataLoader userById)
        {
            return await userById.LoadAsync(notification.ActorId);
        }
    }
}