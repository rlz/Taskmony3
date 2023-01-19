namespace Taskmony.Errors;

public static class DirectionErrors
{
    public static ErrorDetails NotFound =>
        new("Direction not found", "DIRECTION_NOT_FOUND");

    public static ErrorDetails MemberNotFound =>
        new("Member not found", "MEMBER_NOT_FOUND");

    public static ErrorDetails AlreadyDeleted =>
        new("Direction already deleted", "DIRECTION_ALREADY_DELETED");
}