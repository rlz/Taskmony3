using Taskmony.GraphQL.DataLoaders;
using Taskmony.GraphQL.Notifications;
using Taskmony.Models;
using Taskmony.Models.Notifications;
using Taskmony.Services;

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
            .Type<ObjectType<User>>()
            .ResolveWith<Resolvers>(r => r.GetCreatedBy(default!, default!, default!));

        descriptor.Field(d => d.Members)
            .ResolveWith<Resolvers>(r => r.GetMembers(default!, default!, default!));

        descriptor.Field(i => i.Notifications)
            .Type<ListType<NonNullType<NotificationType>>>()
            .Argument("start", a => a.Type<StringType>())
            .Argument("end", a => a.Type<StringType>())
            .ResolveWith<Resolvers>(r => r.GetNotifications(default!, default!, default!, default!, default!));
    }

    private class Resolvers
    {
        public async Task<User> GetCreatedBy([Parent] Direction direction, UserByIdDataLoader userById,
            [GlobalState] Guid currentUserId)
        {
            return await userById.LoadAsync(direction.CreatedById);
        }

        public async Task<IEnumerable<User>?> GetMembers([Parent] Direction direction,
            [Service] IDirectionService directionService, [GlobalState] Guid currentUserId)
        {
            return await directionService.GetDirectionMembersAsync(direction.Id, currentUserId);
        }

        public async Task<IEnumerable<Notification>?> GetNotifications([Parent] Direction direction,
            [Service] INotificationService notificationService, [Service] ITimeConverter timeConverter,
            string? start, string? end)
        {
            DateTime? startDateTime = start is null ? null : timeConverter.StringToDateTimeUtc(start);
            DateTime? endDateTime = end is null ? null : timeConverter.StringToDateTimeUtc(end);

            return await notificationService.GetDirectionNotificationsAsync(direction.Id, startDateTime, endDateTime);
        }
    }
}