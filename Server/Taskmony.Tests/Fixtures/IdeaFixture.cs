using Taskmony.Models.Ideas;
using Taskmony.Models.ValueObjects;

namespace Taskmony.Tests.Fixtures;

public static class IdeaFixture
{
    public static Idea GetIdea(Guid userId, Guid? directionId = null)
    {
        return new Idea(
            description: Description.From("Idea"),
            details: Details.From(null),
            createdById: userId,
            generation: Generation.Hot,
            directionId: directionId);
    }
    
    public static IEnumerable<Idea> GetIdeas(Guid userId)
    {
        var otherUserId = Guid.NewGuid();
        var privateDirectionCreatedByUser = DirectionFixture.GetPrivateUserDirection(userId);
        var privateDirectionCreatedByOtherUser = DirectionFixture.GetPrivateUserDirection(otherUserId);
        var publicDirectionCreatedByUser = DirectionFixture.GetPublicDirectionCreatedByUser(userId);
        var publicDirectionCreatedByOtherUser = DirectionFixture.GetPublicDirectionCreatedByUser(otherUserId);

        return new List<Idea>
        {
            new(Guid.NewGuid(),
                Description.From("Private idea with no direction"),
                Details.From(null),
                otherUserId,
                Generation.Hot,
                null),
            new(Guid.NewGuid(),
                Description.From("Private user idea with no direction"),
                Details.From(null),
                userId,
                Generation.Hot,
                null),
            new(Guid.NewGuid(),
                Description.From("Private user idea"),
                Details.From(null),
                userId,
                Generation.TooGoodToDelete,
                privateDirectionCreatedByUser.Id),
            new(Guid.NewGuid(),
                Description.From("Private idea"),
                Details.From(null),
                otherUserId,
                Generation.Hot,
                privateDirectionCreatedByOtherUser.Id),
            new(Guid.NewGuid(),
                Description.From("Public user idea created by user"),
                Details.From(null),
                userId,
                Generation.Later,
                publicDirectionCreatedByUser.Id),
            new(Guid.NewGuid(),
                Description.From("Public user idea created by other user"),
                Details.From(null),
                otherUserId,
                Generation.Later,
                publicDirectionCreatedByUser.Id),
            new(Guid.NewGuid(),
                Description.From("Public user idea created by user"),
                Details.From(null),
                userId,
                Generation.Later,
                publicDirectionCreatedByOtherUser.Id),
            new(Guid.NewGuid(),
                Description.From("Public user idea created by other user"),
                Details.From(null),
                otherUserId,
                Generation.Later,
                publicDirectionCreatedByOtherUser.Id)
        };
    }
}