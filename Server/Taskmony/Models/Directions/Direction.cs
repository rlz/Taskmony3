using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Ideas;
using Taskmony.Models.Notifications;
using Taskmony.Models.Users;
using Taskmony.ValueObjects;
using Task = Taskmony.Models.Tasks.Task;

namespace Taskmony.Models.Directions;

public class Direction : Entity
{
    public DirectionName? Name { get; private set; }

    public string? Details { get; private set; }

    public User? CreatedBy { get; private set; }

    public Guid CreatedById { get; private set; }

    public DateTime? CreatedAt { get; private set; }

    public DeletedAt? DeletedAt { get; private set; }

    public ICollection<User>? Members { get; private set; }

    public ICollection<Task>? Tasks { get; private set; }

    public ICollection<Idea>? Ideas { get; private set; }

    public ICollection<Notification>? Notifications { get; set; }

    // Required by EF Core
    private Direction()
    {
    }

    public Direction(Guid createdById, DirectionName name, string? details, DateTime? createdAt = null,
        DeletedAt? deletedAt = null)
    {
        CreatedById = createdById;
        Name = name;
        Details = details;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        DeletedAt = deletedAt;
    }

    public void UpdateDeletedAt(DeletedAt? deletedAt)
    {
        if (deletedAt != null && DeletedAt != null)
        {
            throw new DomainException(DirectionErrors.AlreadyDeleted);
        }

        DeletedAt = deletedAt;
    }

    public void UpdateName(DirectionName name)
    {
        ValidateDirectionToUpdate();

        Name = name;
    }

    public void UpdateDetails(string? details)
    {
        ValidateDirectionToUpdate();

        Details = details;
    }

    public void ValidateDirectionToUpdate()
    {
        if (DeletedAt != null)
        {
            throw new DomainException(DirectionErrors.UpdateDeletedDirection);
        }
    }
}