namespace Taskmony.Models.Enums;

public enum ActionType : byte
{
    TaskAdded = 0,
    TaskDeleted = 1,
    TaskAssigned = 2,
    IdeaAdded = 3,
    IdeaDeleted = 4,
    MemberAdded = 5,
    MemberRemoved = 6,
    MemberLeft = 7,
    CommentAdded = 8,
    ItemUpdated = 9
}