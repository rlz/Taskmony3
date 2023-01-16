using Taskmony.Models.Enums;

namespace Taskmony.Models.Notifications;

public class Notification
{
    public Guid Id { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public Guid ActorId { get; set; }

    public User? Actor { get; set; }

    public NotifiableType? NotifiableType { get; set; }

    public Guid NotifiableId { get; set; }

    public ActionType? ActionType { get; set; }

    public ActionItemType? ActionItemType { get; set; }
    
    public IActionItem? ActionItem { get; set; }

    public Guid? ActionItemId { get; set; }

    public string? Field { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }
}