using HotChocolate.Resolvers;
using Taskmony.GraphQL.Comments;
using Taskmony.GraphQL.DataLoaders;
using Taskmony.GraphQL.Notifications;
using Taskmony.GraphQL.Users;
using Taskmony.Models;
using Taskmony.Models.Comments;
using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;
using Taskmony.Services;

namespace Taskmony.GraphQL.Ideas;

public class IdeaType : ObjectType<Idea>
{
    protected override void Configure(IObjectTypeDescriptor<Idea> descriptor)
    {
        descriptor.Field(i => i.CreatedById).Ignore();
        descriptor.Field(i => i.DirectionId).Ignore();
        descriptor.Field(i => i.Subscriptions).Ignore();
        descriptor.Field(i => i.ActionItemType).Ignore();

        descriptor.Field(i => i.ReviewedAt).Type<StringType>();
        descriptor.Field(i => i.Description).Type<StringType>();
        descriptor.Field(i => i.CreatedAt).Type<StringType>();
        descriptor.Field(i => i.DeletedAt).Type<StringType>();
        descriptor.Field(i => i.Generation).Type<EnumType<Generation>>();

        descriptor.Field(i => i.CreatedBy)
            .Type<UserType>()
            .ResolveWith<Resolvers>(r => r.GetCreatedBy(default!, default!));

        descriptor.Field(i => i.Direction)
            .ResolveWith<Resolvers>(r => r.GetDirection(default!, default!));

        descriptor.Field(i => i.Comments)
            .Type<ListType<NonNullType<CommentType>>>()
            .Argument("offset", a => a.Type<IntType>())
            .Argument("limit", a => a.Type<IntType>())
            .ResolveWith<Resolvers>(r => r.GetComments(default!, default!, default!, default, default));

        descriptor.Field(i => i.Notifications)
            .Type<ListType<NonNullType<NotificationType>>>()
            .Argument("start", a => a.Type<StringType>())
            .Argument("end", a => a.Type<StringType>())
            .ResolveWith<Resolvers>(r => r.GetNotifications(default!, default!, default!, default!, default, default));

        //Extension
        descriptor
            .Field("subscribers")
            .Type<ListType<NonNullType<UserType>>>()
            .Argument("offset", a => a.Type<IntType>())
            .Argument("limit", a => a.Type<IntType>())
            .ResolveWith<Resolvers>(r => r.GetSubscribers(default!, default!, default!, default!, default, default));
    }

    private class Resolvers
    {
        public async Task<User> GetCreatedBy([Parent] Idea idea, UserByIdDataLoader userById)
        {
            return await userById.LoadAsync(idea.CreatedById);
        }

        public async Task<Direction?> GetDirection([Parent] Idea idea, DirectionByIdDataLoader directionById)
        {
            if (idea.DirectionId is null)
            {
                return null;
            }

            return await directionById.LoadAsync(idea.DirectionId.Value);
        }

        public async Task<IEnumerable<Comment>?> GetComments([Parent] Idea idea, IResolverContext context,
            [Service] IServiceProvider serviceProvider, int? offset, int? limit)
        {
            return await context.GroupDataLoader<Guid, Comment>(
                async (ids, _) =>
                {
                    await using var scope = serviceProvider.CreateAsyncScope();

                    var comments = await scope.ServiceProvider.GetRequiredService<ICommentService>()
                        .GetCommentsByIdeaIds(ids.ToArray(), offset, limit);

                    return comments.ToLookup(c => ((IdeaComment)c).IdeaId);
                }, "CommentByIdeaId"
            ).LoadAsync(idea.Id);
        }

        public async Task<IEnumerable<User>?> GetSubscribers([Parent] Idea idea, IResolverContext context,
            [Service] IServiceProvider serviceProvider, UserByIdDataLoader userById, int? offset, int? limit)
        {
            var subscriberIds = await context.GroupDataLoader<Guid, Guid>(
                async (ideaIds, _) =>
                {
                    await using var scope = serviceProvider.CreateAsyncScope();

                    return await scope.ServiceProvider.GetRequiredService<ISubscriptionService>()
                        .GetIdeaSubscriberIdsAsync(ideaIds.ToArray(), offset, limit);
                }, "SubscriberIdByIdeaId"
            ).LoadAsync(idea.Id);

            return await userById.LoadAsync(subscriberIds);
        }

        public async Task<IEnumerable<Notification>?> GetNotifications([Parent] Idea idea, IResolverContext context,
            [Service] IServiceProvider serviceProvider, [Service] ITimeConverter timeConverter, string? start,
            string? end)
        {
            DateTime? startUtc = start is null ? null : timeConverter.StringToDateTimeUtc(start);
            DateTime? endUtc = end is null ? null : timeConverter.StringToDateTimeUtc(end);

            return await context.GroupDataLoader<Guid, Notification>(
                async (notifiableIds, _) =>
                {
                    await using var scope = serviceProvider.CreateAsyncScope();

                    var notifications = await scope.ServiceProvider.GetRequiredService<INotificationService>()
                        .GetNotificationsByNotifiableIdsAsync(notifiableIds.ToArray(), startUtc, endUtc);

                    return notifications.ToLookup(n => n.NotifiableId);
                }, "NotificationByIdeaId"
            ).LoadAsync(idea.Id);
        }
    }
}