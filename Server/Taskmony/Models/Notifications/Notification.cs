using Taskmony.Models.Users;

namespace Taskmony.Models.Notifications;

public class Notification : Entity
{
    public DateTime? ModifiedAt { get; private set; }

    public Guid ModifiedById { get; private set; }

    public User? ModifiedBy { get; private set; }

    public NotifiableType? NotifiableType { get; private set; }

    public Guid NotifiableId { get; private set; }

    public ActionType? ActionType { get; private set; }

    public ActionItemType? ActionItemType { get; private set; }

    public IActionItem? ActionItem { get; private set; }

    public Guid? ActionItemId { get; private set; }

    public string? Field { get; private set; }

    public string? OldValue { get; private set; }

    public string? NewValue { get; private set; }

    // Required by EF Core
    private Notification()
    {
    }

    public Notification(ActionType actionType, DateTime? modifiedAt, Guid modifiedById, NotifiableType notifiableType,
        Guid notifiableId)
    {
        ActionType = actionType;
        ModifiedAt = modifiedAt ?? DateTime.UtcNow;
        ModifiedById = modifiedById;
        NotifiableType = notifiableType;
        NotifiableId = notifiableId;
    }

    public Notification(ActionType actionType, DateTime? modifiedAt, Guid modifiedById, NotifiableType notifiableType,
        Guid notifiableId, ActionItemType? actionItemType, Guid? actionItemId) : this(actionType, modifiedAt,
        modifiedById, notifiableType, notifiableId)
    {
        ActionItemType = actionItemType;
        ActionItemId = actionItemId;
    }

    public Notification(ActionType actionType, DateTime? modifiedAt, Guid modifiedById, NotifiableType notifiableType,
        Guid notifiableId, string field, string? oldValue, string? newValue) : this(actionType, modifiedAt,
        modifiedById, notifiableType, notifiableId)
    {
        Field = field;
        OldValue = oldValue;
        NewValue = newValue;
    }
}