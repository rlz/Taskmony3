using Taskmony.Models;
using Taskmony.Models.Enums;
using Taskmony.ValueObjects;
using Task = Taskmony.Models.Task;

namespace Taskmony.Tests.Fixtures;

public static class TaskFixture
{
    public static Task GetTask(Guid userId, Guid? directionId = null, Guid? assigneeId = null, Guid? assignerId = null)
    {
        var task = new Task
        {
            Id = Guid.NewGuid(),
            Description = Description.From("Task"),
            CreatedById = userId,
            DirectionId = directionId,
        };

        if (assigneeId.HasValue)
        {
            task.Assignment = new Assignment
            {
                Id = Guid.NewGuid(),
                AssigneeId = assigneeId.Value,
                AssignedById = assignerId ?? userId,
                TaskId = task.Id
            };
        }

        return task;
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
        var task = new Task
        {
            Id = Guid.NewGuid(),
            Description = Description.From("Recurring task"),
            CreatedById = userId,
            DirectionId = directionId,
            StartAt = startAt,
            GroupId = groupId
        };

        if (repeatMode != null && repeatEvery != null && repeatUntil != null)
        {
            task.RecurrencePattern = new RecurrencePattern
            {
                Id = Guid.NewGuid(),
                RepeatMode = repeatMode.Value,
                RepeatEvery = repeatEvery.Value,
                RepeatUntil = repeatUntil.Value,
                WeekDays = weekDay
            };
        }

        if (assigneeId.HasValue)
        {
            task.Assignment = new Assignment
            {
                Id = Guid.NewGuid(),
                AssigneeId = assigneeId.Value,
                AssignedById = assignerId ?? userId,
                TaskId = task.Id
            };
        }

        return task;
    }

    public static Task GetDailyRecurringTask(Guid userId, Guid? directionId = null, Guid? assigneeId = null, Guid? assignerId = null,
        Guid? groupId = null)
    {
        var task = new Task
        {
            Id = Guid.NewGuid(),
            Description = Description.From("Daily recurring task"),
            CreatedById = userId,
            DirectionId = directionId,
            StartAt = DateTime.UtcNow.Date,
            RecurrencePattern = new RecurrencePattern
            {
                Id = Guid.NewGuid(),
                RepeatMode = RepeatMode.Day,
                RepeatEvery = 1,
                RepeatUntil = DateTime.UtcNow.Date.AddDays(7)
            },
            GroupId = groupId
        };

        if (assigneeId.HasValue)
        {
            task.Assignment = new Assignment
            {
                Id = Guid.NewGuid(),
                AssigneeId = assigneeId.Value,
                AssignedById = assignerId ?? userId,
                TaskId = task.Id
            };
        }

        return task;
    }

    public static List<Task> GetDailyRecurringTaskInstances(Guid userId, Guid? directionId = null, Guid? assigneeId = null,
        Guid? assignerId = null, Guid? groupId = null)
    {
        var instances = new List<Task>();

        for (var i = 0; i < 7; i++)
        {
            var task = new Task
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Daily recurring task"),
                CreatedById = userId,
                DirectionId = directionId,
                StartAt = DateTime.UtcNow.Date.AddDays(i),
                RecurrencePattern = new RecurrencePattern
                {
                    Id = Guid.NewGuid(),
                    RepeatMode = RepeatMode.Day,
                    RepeatEvery = 1,
                    RepeatUntil = DateTime.UtcNow.Date.AddDays(7)
                },
                GroupId = groupId
            };

            if (assigneeId.HasValue)
            {
                task.Assignment = new Assignment
                {
                    Id = Guid.NewGuid(),
                    AssigneeId = assigneeId.Value,
                    AssignedById = assignerId ?? userId,
                    TaskId = task.Id
                };
            }

            instances.Add(task);
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
            var task = new Task
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Daily recurring task"),
                CreatedById = userId,
                DirectionId = directionId,
                StartAt = DateTime.UtcNow.Date.AddDays(i),
                RecurrencePattern = new RecurrencePattern
                {
                    Id = Guid.NewGuid(),
                    RepeatMode = RepeatMode.Day,
                    RepeatEvery = 1,
                    RepeatUntil = DateTime.UtcNow.Date.AddDays(7)
                },
                GroupId = groupId,
                CompletedAt = CompletedAt.From(DateTime.UtcNow)
            };

            if (assigneeId.HasValue)
            {
                task.Assignment = new Assignment
                {
                    Id = Guid.NewGuid(),
                    AssigneeId = assigneeId.Value,
                    AssignedById = assignerId ?? userId,
                    TaskId = task.Id
                };
            }

            instances.Add(task);
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
            var task = new Task
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Daily recurring task"),
                CreatedById = userId,
                DirectionId = directionId,
                StartAt = DateTime.UtcNow.Date.AddDays(i),
                RecurrencePattern = new RecurrencePattern
                {
                    Id = Guid.NewGuid(),
                    RepeatMode = RepeatMode.Day,
                    RepeatEvery = 1,
                    RepeatUntil = DateTime.UtcNow.Date.AddDays(7)
                },
                GroupId = groupId,
                DeletedAt = DeletedAt.From(DateTime.UtcNow)
            };

            if (assigneeId.HasValue)
            {
                task.Assignment = new Assignment
                {
                    Id = Guid.NewGuid(),
                    AssigneeId = assigneeId.Value,
                    AssignedById = assignerId ?? userId,
                    TaskId = task.Id
                };
            }

            instances.Add(task);
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