using Taskmony.Models;
using Taskmony.ValueObjects;

namespace Taskmony.Tests.Fixtures;

public static class IdeaFixture
{
    public static IEnumerable<Idea> GetIdeas(Guid userId)
    {
        var otherUserId = Guid.NewGuid();
        var privateDirectionCreatedByUser = DirectionFixture.GetPrivateUserDirection(userId);
        var privateDirectionCreatedByOtherUser = DirectionFixture.GetPrivateUserDirection(otherUserId);
        var publicDirectionCreatedByUser = DirectionFixture.GetPublicDirectionCreatedByUser(userId, otherUserId);
        var publicDirectionCreatedByOtherUser = DirectionFixture.GetPublicDirectionCreatedByUser(otherUserId, userId);

        return new List<Idea>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Private idea with no direction"),
                CreatedById = otherUserId
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Private user idea with no direction"),
                CreatedById = userId
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Private idea"),
                CreatedById = userId,
                Direction = privateDirectionCreatedByUser,
                DirectionId = privateDirectionCreatedByUser.Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Private idea"),
                CreatedById = otherUserId,
                Direction = privateDirectionCreatedByOtherUser,
                DirectionId = privateDirectionCreatedByOtherUser.Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Public user idea created by user"),
                CreatedById = userId,
                Direction = publicDirectionCreatedByUser,
                DirectionId = publicDirectionCreatedByUser.Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Public user idea created by other user"),
                CreatedById = otherUserId,
                Direction = publicDirectionCreatedByUser,
                DirectionId = publicDirectionCreatedByUser.Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Public user idea created by user"),
                CreatedById = userId,
                Direction = publicDirectionCreatedByOtherUser,
                DirectionId = publicDirectionCreatedByOtherUser.Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Public user idea created by other user"),
                CreatedById = otherUserId,
                Direction = publicDirectionCreatedByOtherUser,
                DirectionId = publicDirectionCreatedByOtherUser.Id
            },
        };
    }
}