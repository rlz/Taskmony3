using Taskmony.Models;
using Taskmony.ValueObjects;

namespace Taskmony.Tests.Fixtures;

public static class DirectionFixture
{
    public static Direction GetDirectionWithoutMembers()
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Name = DirectionName.From("Direction"),
            CreatedById = Guid.NewGuid()
        };
    }
    
    public static Direction GetPrivateUserDirection(Guid userId)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Name = DirectionName.From("Private user direction"),
            CreatedById = userId,
            Members = new List<User>
            {
                new()
                {
                    Id = userId,
                }
            }
        };
    }

    public static Direction GetPublicDirectionCreatedByUser(Guid userId, Guid otherUserId)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Name = DirectionName.From("Public user direction"),
            CreatedById = userId,
            Members = new List<User>
            {
                new()
                {
                    Id = otherUserId,
                },
                new()
                {
                    Id = userId,
                },
            }
        };
    }
}
