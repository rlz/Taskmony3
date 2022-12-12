namespace Taskmony.Models.Enums;

public enum ActionType : byte
{
    TaskAdded,
    TaskDeleted,
    TaskAssigned,
    IdeaAdded,
    IdeaDeleted,
    MemberAdded,
    MemberRemoved,
    MemberLeft,
    CommentAdded,
    ItemUpdated
}