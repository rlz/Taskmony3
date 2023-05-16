namespace Taskmony.Models.Directions;

public class Membership
{
    public Guid DirectionId { get; private set; }

    public Guid UserId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    // Required by EF Core
    private Membership()
    {
    }

    public Membership(Guid directionId, Guid userId, DateTime? createdAt = null)
    {
        DirectionId = directionId;
        UserId = userId;
        CreatedAt = createdAt ?? DateTime.UtcNow;
    }
}