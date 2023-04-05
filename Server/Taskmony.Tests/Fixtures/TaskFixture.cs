using Taskmony.Models.Enums;
using Taskmony.ValueObjects;
using Task = Taskmony.Models.Task;

namespace Taskmony.Tests.Fixtures;

public static class TaskFixture
{
    public static Task GetTask(Guid userId, Guid? directionId = null, Guid? assigneeId = null, Guid? assignerId = null)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Description = Description.From("Task"),
            CreatedById = userId,
            DirectionId = directionId,
            AssigneeId = assigneeId,
            AssignedById = assignerId
        };
    }

    public static Task GetCompletedTask(Guid userId, Guid? directionId = null, Guid? assigneeId = null, Guid? assignerId = null)
    {
        var task = GetTask(userId, directionId, assigneeId, assignerId);
        task.CompletedAt = CompletedAt.From(DateTime.UtcNow);
        return task;
    }

    public static Task GetDeletedTask(Guid userId, Guid? directionId = null, Guid? assigneeId = null, Guid? assignerId = null)
    {
        var task = GetTask(userId, directionId, assigneeId, assignerId);
        task.DeletedAt = DeletedAt.From(DateTime.UtcNow);
        return task;
    }

    public static Task GetRecurringTask(Guid userId, RepeatMode? repeatMode, int? repeatEvery, DateTime? startAt,
        DateTime? repeatUntil, WeekDay? weekDay, Guid? directionId = null, Guid? assigneeId = null, Guid? assignerId = null,
        Guid? groupId = null)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Description = Description.From("Recurring task"),
            CreatedById = userId,
            DirectionId = directionId,
            AssigneeId = assigneeId,
            AssignedById = assignerId,
            RepeatMode = repeatMode,
            RepeatEvery = repeatEvery,
            StartAt = startAt,
            RepeatUntil = repeatUntil,
            WeekDays = weekDay,
            GroupId = groupId
        };
    }

    public static Task GetDailyRecurringTask(Guid userId, Guid? directionId = null, Guid? assigneeId = null, Guid? assignerId = null,
        Guid? groupId = null)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Description = Description.From("Daily recurring task"),
            CreatedById = userId,
            DirectionId = directionId,
            AssigneeId = assigneeId,
            AssignedById = assignerId,
            RepeatMode = RepeatMode.Day,
            RepeatEvery = 1,
            StartAt = DateTime.UtcNow.Date,
            RepeatUntil = DateTime.UtcNow.Date.AddDays(7),
            GroupId = groupId
        };
    }

    public static List<Task> GetDailyRecurringTaskInstances(Guid userId, Guid? directionId = null, Guid? assigneeId = null, 
        Guid? assignerId = null, Guid? groupId = null)
    {
        var instances = new List<Task>();

        for (var i = 0; i < 7; i++)
        {
            instances.Add(new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Daily recurring task"),
                CreatedById = userId,
                DirectionId = directionId,
                AssigneeId = assigneeId,
                AssignedById = assignerId,
                RepeatMode = RepeatMode.Day,
                RepeatEvery = 1,
                StartAt = DateTime.UtcNow.Date.AddDays(i),
                RepeatUntil = DateTime.UtcNow.Date.AddDays(7),
                GroupId = groupId
            });
        }

        return instances;
    }

    public static Task GetCompletedDailyRecurringTask(Guid userId, Guid? directionId = null, Guid? assigneeId = null,
        Guid? assignerId = null, Guid? groupId = null)
    {
        var task = GetDailyRecurringTask(userId, directionId, assigneeId, assignerId, groupId);
        task.CompletedAt = CompletedAt.From(DateTime.UtcNow);
        return task;
    }

    public static List<Task> GetCompletedDailyRecurringTaskInstances(Guid userId, Guid? directionId = null, 
        Guid? assigneeId = null, Guid? assignerId = null, Guid? groupId = null)
    {
        var instances = new List<Task>();

        for (var i = 0; i < 7; i++)
        {
            instances.Add(new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Daily recurring task"),
                CreatedById = userId,
                DirectionId = directionId,
                AssigneeId = assigneeId,
                AssignedById = assignerId,
                RepeatMode = RepeatMode.Day,
                RepeatEvery = 1,
                StartAt = DateTime.UtcNow.Date.AddDays(i),
                RepeatUntil = DateTime.UtcNow.Date.AddDays(7),
                GroupId = groupId,
                CompletedAt = CompletedAt.From(DateTime.UtcNow)
            });
        }

        return instances;
    }

    public static Task GetDeletedDailyRecurringTask(Guid userId, Guid? directionId = null, Guid? assigneeId = null,
        Guid? assignerId = null, Guid? groupId = null)
    {
        var task = GetDailyRecurringTask(userId, directionId, assigneeId, assignerId, groupId);
        task.DeletedAt = DeletedAt.From(DateTime.UtcNow);
        return task;
    }

    public static List<Task> GetDeletedDailyRecurringTaskInstances(Guid userId, Guid? directionId = null, 
        Guid? assigneeId = null, Guid? assignerId = null, Guid? groupId = null)
    {
        var instances = new List<Task>();

        for (var i = 0; i < 7; i++)
        {
            instances.Add(new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Daily recurring task"),
                CreatedById = userId,
                DirectionId = directionId,
                AssigneeId = assigneeId,
                AssignedById = assignerId,
                RepeatMode = RepeatMode.Day,
                RepeatEvery = 1,
                StartAt = DateTime.UtcNow.Date.AddDays(i),
                RepeatUntil = DateTime.UtcNow.Date.AddDays(7),
                GroupId = groupId,
                DeletedAt = DeletedAt.From(DateTime.UtcNow)
            });
        }

        return instances;
    }

    public static List<Task> GetTasks(Guid userId)
    {
        var otherUserId = Guid.NewGuid();
        var privateDirectionCreatedByUser = DirectionFixture.GetPrivateUserDirection(userId);
        var privateDirectionCreatedByOtherUser = DirectionFixture.GetPrivateUserDirection(otherUserId);
        var publicDirectionCreatedByUser = DirectionFixture.GetPublicDirectionCreatedByUser(userId, otherUserId);
        var publicDirectionCreatedByOtherUser = DirectionFixture.GetPublicDirectionCreatedByUser(otherUserId, userId);

        return new List<Task>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Private task with no direction"),
                CreatedById = otherUserId
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Private user task with no direction"),
                CreatedById = userId
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Private task"),
                CreatedById = userId,
                Direction = privateDirectionCreatedByUser,
                DirectionId = privateDirectionCreatedByUser.Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Private task"),
                CreatedById = otherUserId,
                Direction = privateDirectionCreatedByOtherUser,
                DirectionId = privateDirectionCreatedByOtherUser.Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Public user task created by user"),
                CreatedById = userId,
                Direction = publicDirectionCreatedByUser,
                DirectionId = publicDirectionCreatedByUser.Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Public user task created by other user"),
                CreatedById = otherUserId,
                Direction = publicDirectionCreatedByUser,
                DirectionId = publicDirectionCreatedByUser.Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Public user task created by user"),
                CreatedById = userId,
                Direction = publicDirectionCreatedByOtherUser,
                DirectionId = publicDirectionCreatedByOtherUser.Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Public user task created by other user"),
                CreatedById = otherUserId,
                Direction = publicDirectionCreatedByOtherUser,
                DirectionId = publicDirectionCreatedByOtherUser.Id
            },
        };
    }
}