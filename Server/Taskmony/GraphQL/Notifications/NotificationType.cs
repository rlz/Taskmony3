using Taskmony.GraphQL.DataLoaders;
using Taskmony.GraphQL.Users;
using Taskmony.Models.Comments;
using Taskmony.Models.Notifications;
using Taskmony.Models.Users;

namespace Taskmony.GraphQL.Notifications;

public class NotificationType : ObjectType<Notification>
{
    protected override void Configure(IObjectTypeDescriptor<Notification> descriptor)
    {
        descriptor.Field(n => n.ModifiedById).Ignore();
        descriptor.Field(n => n.NotifiableId).Ignore();
        descriptor.Field(n => n.NotifiableType).Ignore();
        descriptor.Field(n => n.ActionItemId).Ignore();
        descriptor.Field(n => n.ActionItemType).Ignore();

        descriptor.Field(n => n.ModifiedAt).Type<StringType>();
        descriptor.Field(n => n.ActionType).Type<EnumType<ActionType>>();

        descriptor.Field(n => n.ActionItem)
            .ResolveWith<Resolvers>(r => r.GetActionItem(default!, default!, default!, default!, default!))
            .Type<ActionItem>();

        descriptor.Field(n => n.ModifiedBy)
            .Type<UserType>()
            .ResolveWith<Resolvers>(r => r.GetModifiedBy(default!, default!));
    }

    private class Resolvers
    {
        public async Task<IActionItem?> GetActionItem([Parent] Notification notification, UserByIdDataLoader userById,
            TaskByIdDataLoader taskById, IdeaByIdDataLoader ideaById, CommentByIdDataLoader commentById)
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
                ActionItemType.Comment => await GetCommentAsync(notification.ActionItemId.Value, commentById),
                _ => null
            };
        }

        private async Task<Comment> GetCommentAsync(Guid id, CommentByIdDataLoader commentById)
        {
            var concreteComment = await commentById.LoadAsync(id);

            // TODO: This is a workaround that needs to be fixed
            return new Comment(concreteComment.Id, concreteComment.Text!, concreteComment.CreatedById,
                concreteComment.CreatedAt, concreteComment.DeletedAt);
        }

        public async Task<User> GetModifiedBy([Parent] Notification notification, UserByIdDataLoader userById)
        {
            return await userById.LoadAsync(notification.ModifiedById);
        }
    }
}