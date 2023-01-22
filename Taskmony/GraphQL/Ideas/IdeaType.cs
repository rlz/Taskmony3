using HotChocolate.Resolvers;
using Taskmony.GraphQL.DataLoaders;
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
            .Type<ObjectType<User>>()
            .ResolveWith<Resolvers>(r => r.GetCreatedBy(default!, default!, default!));

        descriptor.Field(i => i.Direction)
            .ResolveWith<Resolvers>(r => r.GetDirection(default!, default!));

        descriptor.Field(i => i.Comments)
            .Type<ListType<NonNullType<ObjectType<Comment>>>>()
            .Argument("offset", a => a.Type<IntType>())
            .Argument("limit", a => a.Type<IntType>())
            .ResolveWith<Resolvers>(r => r.GetComments(default!, default!, default!, default, default));

        descriptor.Field(i => i.Notifications)
            .Type<ListType<NonNullType<ObjectType<Notification>>>>()
            .Argument("start", a => a.Type<StringType>())
            .Argument("end", a => a.Type<StringType>())
            .ResolveWith<Resolvers>(r => r.GetNotifications(default!, default!, default!, default!, default, default));

        //Extension
        descriptor
            .Field("subscribers")
            .Type<ListType<NonNullType<UserType>>>()
            .ResolveWith<Resolvers>(r => r.GetSubscribers(default!, default!, default!));
    }

    private class Resolvers
    {
        public async Task<User> GetCreatedBy([Parent] Idea idea, UserByIdDataLoader userById,
            [GlobalState] Guid currentUserId)
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
            IServiceProvider serviceProvider, int? offset, int? limit)
        {
            return await context.GroupDataLoader<Guid, Comment>(
                async (keys, ct) =>
                {
                    await using var scope = serviceProvider.CreateAsyncScope();

                    var commentService = scope.ServiceProvider.GetRequiredService<ICommentService>();

                    var comments = await commentService.GetCommentsByIdeaIds(keys.ToArray(), offset, limit);

                    return comments.ToLookup(c => ((IdeaComment)c).IdeaId);
                }
            ).LoadAsync(idea.Id);
        }

        public async Task<IEnumerable<User>?> GetSubscribers([Parent] Idea idea,
            [Service] ISubscriptionService subscriptionService, [GlobalState] Guid currentUserId)
        {
            return await subscriptionService.GetIdeaSubscribersAsync(idea.Id, currentUserId);
        }

        public async Task<IEnumerable<Notification>?> GetNotifications([Parent] Idea idea, IResolverContext context,
            [Service] IServiceProvider serviceProvider, [Service] ITimeConverter timeConverter, string? start, string? end)
        {
            DateTime? startUtc = start is null ? null : timeConverter.StringToDateTimeUtc(start);
            DateTime? endUtc = end is null ? null : timeConverter.StringToDateTimeUtc(end);

            return await context.GroupDataLoader<Guid, Notification>(
                async (keys, ct) =>
                {
                    await using var scope = serviceProvider.CreateAsyncScope();

                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                    var notifications =
                        await notificationService.GetNotificationsByNotifiableIdsAsync(keys.ToArray(), startUtc, endUtc);
                        
                    return notifications.ToLookup(n => n.NotifiableId);
                }
            ).LoadAsync(idea.Id);
        }
    }
}