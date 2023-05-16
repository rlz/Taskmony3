using Taskmony.Models.Notifications;

namespace Taskmony.Models.Directions;

public abstract class DirectionEntity : Entity, IActionItem
{
    public Direction? Direction { get; protected set; }

    public Guid? DirectionId { get; protected set; }

    public abstract ActionItemType ActionItemType { get; }
}