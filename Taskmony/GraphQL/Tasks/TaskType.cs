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
        public async Task<User> GetCreatedBy([Parent] Task task, [Service] IUserService userService,
            [GlobalState] Guid userId)
        {
            var result = await userService.GetUsersAsync(new[] {task.CreatedById}, 
                null, null, null, null, userId);
                
            return result.First();
        }

        public async Task<User?> GetAssignee([Parent] Task task, [Service] IUserService userService,
            [GlobalState] Guid userId)
        {
            if (task.AssigneeId is null)
            {
                return null;
            }

            var result = await userService.GetUsersAsync(new[] {task.AssigneeId.Value},
                null, null, null, null, userId);

            return result.First();
        }

        public async Task<Direction?> GetDirection([Parent] Task task, [Service] IDirectionService directionService)
        {
            if (task.DirectionId is null)
            {
                return null;
            }

            return await directionService.GetDirectionByIdAsync(task.DirectionId.Value);
        }

        public async Task<IEnumerable<Comment>?> GetComments([Parent] Task task,
            [Service] ICommentService commentService, int? offset, int? limit)
        {
            return await commentService.GetTaskCommentsAsync(task.Id, offset, limit);
        }

        public async Task<IEnumerable<User>?> GetSubscribers([Parent] Task task,
            [Service] ISubscriptionService subscriptionService, [GlobalState] Guid userId)
        {
            return await subscriptionService.GetTaskSubscribersAsync(task.Id, userId);
        }

        public async Task<IEnumerable<Notification>?> GetNotifications([Parent] Task task,
            [Service] INotificationService notificationService, [Service] ITimeConverter timeConverter,
            string? start, string? end)
        {
            DateTime? startDateTime = start is null ? null : timeConverter.Rfc3339ToDateTimeUtc(start);
            DateTime? endDateTime = end is null ? null : timeConverter.Rfc3339ToDateTimeUtc(end);

            return await notificationService.GetTaskNotificationsAsync(task.Id, startDateTime, endDateTime);
        }
    }
}