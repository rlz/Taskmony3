using HotChocolate.Resolvers;
using Taskmony.GraphQL.Comments;
using Taskmony.GraphQL.DataLoaders;
using Taskmony.GraphQL.Notifications;
using Taskmony.GraphQL.Users;
using Taskmony.Models;
using Taskmony.Models.Comments;
using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;
using Taskmony.Services.Abstract;
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
        descriptor.Field(t => t.RepeatUntil).Type<StringType>();
        descriptor.Field(t => t.GroupId).Type<IdType>();

        descriptor.Field(t => t.WeekDays)
            .Type<ListType<NonNullType<EnumType<WeekDay>>>>()
            .ResolveWith<Resolvers>(r => r.GetWeekDays(default!));

        descriptor.Field(t => t.Direction)
            .ResolveWith<Resolvers>(r => r.GetDirection(default!, default!));

        descriptor.Field(t => t.CreatedBy)
            .Type<UserType>()
            .ResolveWith<Resolvers>(r => r.GetCreatedBy(default!, default!));

        descriptor.Field(t => t.Assignee)
            .ResolveWith<Resolvers>(r => r.GetAssignee(default!, default!));

        descriptor.Field(t => t.Comments)
            .Type<ListType<NonNullType<CommentType>>>()
            .Argument("offset", a => a.Type<IntType>())
            .Argument("limit", a => a.Type<IntType>())
            .ResolveWith<Resolvers>(r => r.GetComments(default!, default!, default!, default, default));

        //Extension
        descriptor
            .Field("subscribers")
            .Type<ListType<NonNullType<UserType>>>()
            .Argument("offset", a => a.Type<IntType>())
            .Argument("limit", a => a.Type<IntType>())
            .ResolveWith<Resolvers>(r => r.GetSubscribers(default!, default!, default!, default!, default, default));

        descriptor.Field(i => i.Notifications)
            .Type<ListType<NonNullType<NotificationType>>>()
            .Argument("start", a => a.Type<StringType>())
            .Argument("end", a => a.Type<StringType>())
            .ResolveWith<Resolvers>(r =>
                r.GetNotifications(default!, default!, default!, default!, default, default));
    }

    private class Resolvers
    {
        public async Task<User> GetCreatedBy([Parent] Task task, UserByIdDataLoader userById)
        {
            return await userById.LoadAsync(task.CreatedById);
        }

        public async Task<User?> GetAssignee([Parent] Task task, UserByIdDataLoader userById)
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

        public async Task<IEnumerable<Comment>?> GetComments([Parent] Task task, IResolverContext context,
            [Service] IServiceProvider serviceProvider, int? offset, int? limit)
        {
            return await context.GroupDataLoader<Guid, Comment>(
                async (ids, _) =>
                {
                    await using var scope = serviceProvider.CreateAsyncScope();

                    var comments = await scope.ServiceProvider.GetRequiredService<ICommentService>()
                        .GetCommentsByTaskIds(ids.ToArray(), offset, limit);

                    return comments.ToLookup(c => ((TaskComment)c).TaskId);
                }, "CommentByTaskId"
            ).LoadAsync(task.Id);
        }

        public async Task<IEnumerable<User>?> GetSubscribers([Parent] Task task, IResolverContext context,
            UserByIdDataLoader userById, [Service] IServiceProvider serviceProvider, int? offset, int? limit)
        {
            var subscriberIds = await context.GroupDataLoader<Guid, Guid>(
                async (taskIds, _) =>
                {
                    await using var scope = serviceProvider.CreateAsyncScope();

                    return await scope.ServiceProvider.GetRequiredService<ISubscriptionService>()
                        .GetTaskSubscriberIdsAsync(taskIds.ToArray(), offset, limit);
                }, "SubscriberIdByTaskId"
            ).LoadAsync(task.Id);

            return await userById.LoadAsync(subscriberIds);
        }

        public async Task<IEnumerable<Notification>?> GetNotifications([Parent] Task task, IResolverContext context,
            [Service] IServiceProvider serviceProvider, [Service] ITimeConverter timeConverter,
            string? start, string? end)
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
                }, "NotificationByTaskId"
            ).LoadAsync(task.Id);
        }

        public IEnumerable<WeekDay>? GetWeekDays([Parent] Task task)
        {
            if (task.WeekDays is null)
            {
                return null;
            }

            return Enum.GetValues(typeof(WeekDay))
                .Cast<WeekDay>()
                .Where(f => (f & task.WeekDays.Value) == f)
                .ToList();
        }
    }
}