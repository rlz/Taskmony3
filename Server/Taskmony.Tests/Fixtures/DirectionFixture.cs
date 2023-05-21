using Taskmony.Models.Directions;
using Taskmony.Models.ValueObjects;

namespace Taskmony.Tests.Fixtures;

public static class DirectionFixture
{
    public static Direction GetUserDirection(Guid userId)
    {
        return new Direction(
            Guid.NewGuid(),
            userId,
            DirectionName.From("Direction"),
            Details.From(null));
    }

    public static Direction GetPrivateUserDirection(Guid userId)
    {
        return new Direction(
            Guid.NewGuid(),
            userId,
            DirectionName.From("Private user direction"),
            Details.From(null));
    }

    public static Direction GetPublicDirectionCreatedByUser(Guid userId)
    {
        return new Direction(
            Guid.NewGuid(),
            userId,
            DirectionName.From("Public user direction"),
            Details.From(null));
    }
}