using HotChocolate.Resolvers;
using Taskmony.GraphQL.DataLoaders;
using Taskmony.GraphQL.Notifications;
using Taskmony.GraphQL.Users;
using Taskmony.Models;
using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;
using Taskmony.Services.Abstract;

namespace Taskmony.GraphQL.Directions;

public class DirectionType : ObjectType<Direction>
{
    protected override void Configure(IObjectTypeDescriptor<Direction> descriptor)
    {
        descriptor.Field(d => d.CreatedById).Ignore();
        descriptor.Field(d => d.Ideas).Ignore();
        descriptor.Field(d => d.Tasks).Ignore();

        descriptor.Field(d => d.Name).Type<StringType>();
        descriptor.Field(d => d.CreatedAt).Type<StringType>();
        descriptor.Field(d => d.DeletedAt).Type<StringType>();

        descriptor.Field(d => d.CreatedBy)
            .Type<UserType>()
            .ResolveWith<Resolvers>(r => r.GetCreatedBy(default!, default!));

        descriptor.Field(d => d.Members)
            .Type<ListType<NonNullType<UserType>>>()
            .Argument("offset", a => a.Type<IntType>())
            .Argument("limit", a => a.Type<IntType>())
            .ResolveWith<Resolvers>(r => r.GetMembers(default!, default!, default!, default!, default, default));

        descriptor.Field(i => i.Notifications)
            .Type<ListType<NonNullType<NotificationType>>>()
            .Argument("start", a => a.Type<StringType>())
            .Argument("end", a => a.Type<StringType>())
            .ResolveWith<Resolvers>(r => r.GetNotifications(default!, default!, default!, default!, default!, default, default));
    }

    private class Resolvers
    {
        public async Task<User> GetCreatedBy([Parent] Direction direction, UserByIdDataLoader userById)
        {
            return await userById.LoadAsync(direction.CreatedById);
        }

        public async Task<IEnumerable<User>?> GetMembers([Parent] Direction direction, IResolverContext context,
            UserByIdDataLoader userById, [Service] IServiceProvider serviceProvider, int? offset, int? limit)
        {
            var memberIds = await context.GroupDataLoader<Guid, Guid>(
                async (directionIds, _) =>
                {
                    await using var scope = serviceProvider.CreateAsyncScope();

                    return await scope.ServiceProvider.GetRequiredService<IDirectionService>()
                        .GetMemberIdsAsync(directionIds.ToArray(), offset, limit);
                }, "MemberIdByDirectionId"
            ).LoadAsync(direction.Id);

            return await userById.LoadAsync(memberIds);
        }

        public async Task<IEnumerable<Notification>?> GetNotifications([Parent] Direction direction,
            [GlobalState] Guid currentUserId, IResolverContext context, [Service] IServiceProvider serviceProvider,
            [Service] ITimeConverter timeConverter, string? start, string? end)
        {
            DateTime? startUtc = start is null ? null : timeConverter.StringToDateTimeUtc(start);
            DateTime? endUtc = end is null ? null : timeConverter.StringToDateTimeUtc(end);

            return await context.GroupDataLoader<Guid, Notification>(
                async (notifiableIds, _) =>
                {
                    await using var scope = serviceProvider.CreateAsyncScope();

                    var notifications = await scope.ServiceProvider.GetRequiredService<INotificationService>()
                        .GetNotificationsByNotifiableIdsAsync(NotifiableType.Direction, notifiableIds.ToArray(), startUtc, endUtc, currentUserId);

                    return notifications.ToLookup(n => n.NotifiableId);
                }, "NotificationByDirectionId"
            ).LoadAsync(direction.Id);
        }
    }
}