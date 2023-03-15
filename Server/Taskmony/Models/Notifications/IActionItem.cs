using Taskmony.Models.Enums;

namespace Taskmony.Models.Notifications;

public interface IActionItem
{
    public ActionItemType ActionItemType { get; }
}