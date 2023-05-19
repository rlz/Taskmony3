// using Taskmony.Models.Tasks;
// using Taskmony.Models.ValueObjects;
// using Task = Taskmony.Models.Tasks.Task;
//
// namespace Taskmony.Tests.Fixtures;
//
// public static class TaskFixture
// {
//     public static Task GetTask(Guid userId, Guid? directionId = null, Guid? assigneeId = null, Guid? assignerId = null)
//     {
//         var task = new Task(
//             id: Guid.NewGuid(),
//             description: Description.From("Task"),
//             details: null,
//             createdById: userId,
//             startAt: DateTime.UtcNow,
//             assignment: assigneeId != null ? new Assignment(assigneeId.Value, assignerId ?? userId) : null,
//             directionId: directionId);
//
//         return task;
//     }
//
//     public static Task GetCompletedTask(Guid userId, Guid? directionId = null, Guid? assigneeId = null,
//         Guid? assignerId = null)
//     {
//         var task = GetTask(userId, directionId, assigneeId, assignerId);
//         task.UpdateCompletedAt(CompletedAt.From(DateTime.UtcNow));
//         return task;
//     }
//
//     public static Task GetDeletedTask(Guid userId, Guid? directionId = null, Guid? assigneeId = null,
//         Guid? assignerId = null)
//     {
//         var task = GetTask(userId, directionId, assigneeId, assignerId);
//         task.UpdateDeletedAt(DeletedAt.From(DateTime.UtcNow));
//         return task;
//     }
//
//     public static Task GetRecurringTask(Guid userId, RepeatMode? repeatMode, int? repeatEvery, DateTime? startAt,
//         DateTime? repeatUntil, WeekDay? weekDay, Guid? directionId = null, Guid? assigneeId = null,
//         Guid? assignerId = null,
//         Guid? groupId = null)
//     {
//         var recurrencePattern = repeatMode != null && repeatEvery != null && repeatUntil != null
//             ? new RecurrencePattern(repeatMode.Value, weekDay, repeatEvery.Value, repeatUntil.Value)
//             : null;
//
//         var task = new Task(
//             id: Guid.NewGuid(),
//             description: Description.From("Recurring task"),
//             details: null,
//             createdById: userId,
//             startAt: startAt,
//             assignment: assigneeId != null ? new Assignment(assigneeId.Value, assignerId ?? userId) : null,
//             directionId: directionId,
//             recurrencePattern: recurrencePattern,
//             groupId: groupId);
//
//         return task;
//     }
//
//     public static Task GetDailyRecurringTask(Guid userId, Guid? directionId = null, Guid? assigneeId = null,
//         Guid? assignerId = null,
//         Guid? groupId = null)
//     {
//         var task = new Task(
//             id: Guid.NewGuid(),
//             description: Description.From("Daily recurring task"),
//             details: null,
//             createdById: userId,
//             startAt: DateTime.UtcNow.Date,
//             assignment: assigneeId != null ? new Assignment(assigneeId.Value, assignerId ?? userId) : null,
//             directionId: directionId,
//             recurrencePattern: new RecurrencePattern(RepeatMode.Day, null, 1, DateTime.UtcNow.Date.AddDays(7)),
//             groupId: groupId);
//
//         return task;
//     }
//
//     public static List<Task> GetDailyRecurringTaskInstances(Guid userId, Guid? directionId = null,
//         Guid? assigneeId = null,
//         Guid? assignerId = null, Guid? groupId = null)
//     {
//         var instances = new List<Task>();
//
//         for (var i = 0; i < 7; i++)
//         {
//             var task = new Task(
//                 id: Guid.NewGuid(),
//                 description: Description.From("Daily recurring task"),
//                 details: null,
//                 createdById: userId,
//                 startAt: DateTime.UtcNow.Date.AddDays(i),
//                 assignment: assigneeId != null ? new Assignment(assigneeId.Value, assignerId ?? userId) : null,
//                 directionId: directionId,
//                 recurrencePattern: new RecurrencePattern(RepeatMode.Day, null, 1, DateTime.UtcNow.Date.AddDays(7)),
//                 groupId: groupId);
//
//             instances.Add(task);
//         }
//
//         return instances;
//     }
//
//     public static Task GetCompletedDailyRecurringTask(Guid userId, Guid? directionId = null, Guid? assigneeId = null,
//         Guid? assignerId = null, Guid? groupId = null)
//     {
//         var task = GetDailyRecurringTask(userId, directionId, assigneeId, assignerId, groupId);
//         task.UpdateCompletedAt(CompletedAt.From(DateTime.UtcNow));
//         return task;
//     }
//
//     public static List<Task> GetCompletedDailyRecurringTaskInstances(Guid userId, Guid? directionId = null,
//         Guid? assigneeId = null, Guid? assignerId = null, Guid? groupId = null)
//     {
//         var instances = new List<Task>();
//
//         for (var i = 0; i < 7; i++)
//         {
//             var task = new Task(
//                 id: Guid.NewGuid(),
//                 description: Description.From("Daily recurring task"),
//                 details: null,
//                 createdById: userId,
//                 startAt: DateTime.UtcNow.Date.AddDays(i),
//                 assignment: assigneeId != null ? new Assignment(assigneeId.Value, assignerId ?? userId) : null,
//                 directionId: directionId,
//                 recurrencePattern: new RecurrencePattern(RepeatMode.Day, null, 1, DateTime.UtcNow.Date.AddDays(7)),
//                 groupId: groupId,
//                 completedAt: CompletedAt.From(DateTime.UtcNow));
//
//             instances.Add(task);
//         }
//
//         return instances;
//     }
//
//     public static Task GetDeletedDailyRecurringTask(Guid userId, Guid? directionId = null, Guid? assigneeId = null,
//         Guid? assignerId = null, Guid? groupId = null)
//     {
//         var task = GetDailyRecurringTask(userId, directionId, assigneeId, assignerId, groupId);
//         task.UpdateDeletedAt(DeletedAt.From(DateTime.UtcNow));
//         return task;
//     }
//
//     public static List<Task> GetDeletedDailyRecurringTaskInstances(Guid userId, Guid? directionId = null,
//         Guid? assigneeId = null, Guid? assignerId = null, Guid? groupId = null)
//     {
//         var instances = new List<Task>();
//
//         for (var i = 0; i < 7; i++)
//         {
//             var task = new Task(
//                 id: Guid.NewGuid(),
//                 description: Description.From("Daily recurring task"),
//                 details: null,
//                 createdById: userId,
//                 startAt: DateTime.UtcNow.Date.AddDays(i),
//                 assignment: assigneeId != null ? new Assignment(assigneeId.Value, assignerId ?? userId) : null,
//                 directionId: directionId,
//                 recurrencePattern: new RecurrencePattern(RepeatMode.Day, null, 1, DateTime.UtcNow.Date.AddDays(7)),
//                 groupId: groupId,
//                 deletedAt: DeletedAt.From(DateTime.UtcNow));
//
//             instances.Add(task);
//         }
//
//         return instances;
//     }
//
//     public static List<Task> GetTasks(Guid userId)
//     {
//         var otherUserId = Guid.NewGuid();
//         var privateDirectionCreatedByUser = DirectionFixture.GetPrivateUserDirection(userId);
//         var privateDirectionCreatedByOtherUser = DirectionFixture.GetPrivateUserDirection(otherUserId);
//         var publicDirectionCreatedByUser = DirectionFixture.GetPublicDirectionCreatedByUser(userId, otherUserId);
//         var publicDirectionCreatedByOtherUser = DirectionFixture.GetPublicDirectionCreatedByUser(otherUserId, userId);
//
//         return new List<Task>
//         {
//             new(id: Guid.NewGuid(),
//                 startAt: DateTime.UtcNow.Date,
//                 description: Description.From("Private task with no direction"),
//                 details: null,
//                 assignment: null,
//                 createdById: otherUserId),
//             new(id: Guid.NewGuid(),
//                 startAt: DateTime.UtcNow.Date,
//                 description: Description.From("Private user task with no direction"),
//                 details: null,
//                 assignment: null,
//                 createdById: userId),
//             new(id: Guid.NewGuid(),
//                 startAt: DateTime.UtcNow.Date,
//                 description: Description.From("Private user task"),
//                 details: null,
//                 assignment: null,
//                 createdById: userId,
//                 directionId: privateDirectionCreatedByUser.Id,
//                 direction: privateDirectionCreatedByUser),
//             new(id: Guid.NewGuid(),
//                 startAt: DateTime.UtcNow.Date,
//                 description: Description.From("Private task"),
//                 details: null,
//                 assignment: null,
//                 createdById: otherUserId,
//                 directionId: privateDirectionCreatedByOtherUser.Id,
//                 direction: privateDirectionCreatedByOtherUser),
//             new(id: Guid.NewGuid(),
//                 startAt: DateTime.UtcNow.Date,
//                 description: Description.From("Public user task created by user"),
//                 details: null,
//                 assignment: null,
//                 createdById: userId,
//                 directionId: publicDirectionCreatedByUser.Id,
//                 direction: publicDirectionCreatedByUser),
//             new(id: Guid.NewGuid(),
//                 startAt: DateTime.UtcNow.Date,
//                 description: Description.From("Public user task created by other user"),
//                 details: null,
//                 assignment: null,
//                 createdById: otherUserId,
//                 directionId: publicDirectionCreatedByUser.Id,
//                 direction: publicDirectionCreatedByUser),
//             new(id: Guid.NewGuid(),
//                 startAt: DateTime.UtcNow.Date,
//                 description: Description.From("Public user task created by user"),
//                 details: null,
//                 assignment: null,
//                 createdById: userId,
//                 directionId: publicDirectionCreatedByOtherUser.Id,
//                 direction: publicDirectionCreatedByOtherUser),
//             new(id: Guid.NewGuid(),
//                 startAt: DateTime.UtcNow.Date,
//                 description: Description.From("Public user task created by other user"),
//                 details: null,
//                 assignment: null,
//                 createdById: otherUserId,
//                 directionId: publicDirectionCreatedByOtherUser.Id,
//                 direction: publicDirectionCreatedByOtherUser)
//         };
//     }
// }