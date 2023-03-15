using Taskmony.ValueObjects;
using Task = Taskmony.Models.Task;

namespace Taskmony.Tests.Fixtures;

public static class TaskFixture
{
    public static IEnumerable<Task> GetTasks(Guid userId)
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