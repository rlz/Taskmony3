namespace Taskmony.Models.Notifications;

public enum ActionType : byte
{
    ItemAdded = 0,
    ItemUpdated = 1,
    ItemDeleted = 2,
    TaskAssigned = 3
}