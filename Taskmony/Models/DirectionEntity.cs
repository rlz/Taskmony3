using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;

namespace Taskmony.Models;

public abstract class DirectionEntity : Entity, IActionItem
{
    public Direction? Direction { get; set; }
    
    public Guid? DirectionId { get; set; }
    
    public abstract ActionItemType ActionItemType { get; }
}