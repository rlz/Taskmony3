using Taskmony.Models;
using Taskmony.ValueObjects;

namespace Taskmony.Tests.Fixtures;

public static class TasksFixture
{
    public static List<Models.Task> GetTasks(Guid userId)
    {
        return new List<Models.Task>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Private task with no direction"),
                CreatedById = Guid.NewGuid()
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
                Direction = new()
                {
                    Id = Guid.NewGuid(),
                    Name = DirectionName.From("Private user direction"),
                    Members = new List<User>
                    {
                        new()
                        {
                            Id = userId,
                        },
                    }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Private task"),
                CreatedById = Guid.NewGuid(),
                Direction = new()
                {
                    Id = Guid.NewGuid(),
                    Name = DirectionName.From("Private direction"),
                    Members = new List<User>
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                        },
                    }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = Description.From("Public user task"),
                CreatedById = userId,
                Direction = new()
                {
                    Id = Guid.NewGuid(),
                    Name = DirectionName.From("Public direction with user"),
                    Members = new List<User>
                    {
                        new()
                        {
                            Id = userId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                        },
                    }
                }
            },
        };
    }
}
