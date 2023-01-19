using Taskmony.GraphQL.DataLoaders;
using Taskmony.GraphQL.Notifications;
using Taskmony.GraphQL.Users;
using Taskmony.Models;
using Taskmony.Models.Comments;
using Taskmony.Models.Notifications;
using Taskmony.Services;
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
        descriptor.Field(t => t.ActionItemType).Ignore();

        descriptor.Field(t => t.CompletedAt).Type<StringType>();
        descriptor.Field(t => t.Description).Type<StringType>();
        descriptor.Field(t => t.CreatedAt).Type<StringType>();
        descriptor.Field(t => t.DeletedAt).Type<StringType>();
        descriptor.Field(t => t.StartAt).Type<StringType>();
        descriptor.Field(t => t.GroupId).Type<IdType>();

        descriptor.Field(t => t.Direction)
            .ResolveWith<Resolvers>(r => r.GetDirection(default!, default!));

        descriptor.Field(t => t.CreatedBy)
            .Type<ObjectType<User>>()
            .ResolveWith<Resolvers>(r => r.GetCreatedBy(default!, default!, default!));

        descriptor.Field(t => t.Assignee)
            .ResolveWith<Resolvers>(r => r.GetAssignee(default!, default!, default!));

        descriptor.Field(t => t.Comments)
            .Type<ListType<NonNullType<ObjectType<Comment>>>>()
            .Argument("offset", a => a.Type<IntType>())
            .Argument("limit", a => a.Type<IntType>())
            .ResolveWith<Resolvers>(r => r.GetComments(default!, default!, default!, default!));

        //Extension
        descriptor
            .Field("subscribers")
            .Type<ListType<NonNullType<UserType>>>()
            .ResolveWith<Resolvers>(r => r.GetSubscribers(default!, default!, default!));

        descriptor.Field(i => i.Notifications)
            .Type<ListType<NonNullType<NotificationType>>>()
            .Argument("start", a => a.Type<StringType>())
            .Argument("end", a => a.Type<StringType>())
            .ResolveWith<Resolvers>(r => r.GetNotifications(default!, default!, default!, default!, default!));
    }

    private class Resolvers
    {
        public async Task<User> GetCreatedBy([Parent] Task task, UserByIdDataLoader userById,
            [GlobalState] Guid currentUserId)
        {
            return await userById.LoadAsync(currentUserId);
        }

        public async Task<User?> GetAssignee([Parent] Task task, UserByIdDataLoader userById,
            [GlobalState] Guid currentUserId)
        {
            if (task.AssigneeId is null)
            {
                return null;
            }

            return await userById.LoadAsync(task.AssigneeId.Value);
        }

        public async Task<Direction?> GetDirection([Parent] Task task, DirectionByIdDataLoader directionById)
        {
            if (task.DirectionId is null)
            {
                return null;
            }

            return await directionById.LoadAsync(task.DirectionId.Value);
        }

        public async Task<IEnumerable<Comment>?> GetComments([Parent] Task task,
            [Service] ICommentService commentService, int? offset, int? limit)
        {
            return await commentService.GetTaskCommentsAsync(task.Id, offset, limit);
        }

        public async Task<IEnumerable<User>?> GetSubscribers([Parent] Task task,
            [Service] ISubscriptionService subscriptionService, [GlobalState] Guid currentUserId)
        {
            return await subscriptionService.GetTaskSubscribersAsync(task.Id, currentUserId);
        }

        public async Task<IEnumerable<Notification>?> GetNotifications([Parent] Task task,
            [Service] INotificationService notificationService, [Service] ITimeConverter timeConverter,
            string? start, string? end)
        {
            DateTime? startDateTime = start is null ? null : timeConverter.StringToDateTimeUtc(start);
            DateTime? endDateTime = end is null ? null : timeConverter.StringToDateTimeUtc(end);

            return await notificationService.GetTaskNotificationsAsync(task.Id, startDateTime, endDateTime);
        }
    }
}