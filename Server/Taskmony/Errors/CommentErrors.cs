namespace Taskmony.Errors;

public static class CommentErrors
{
    public static ErrorDetails NotFound => new("Comment not found", "COMMENT_NOT_FOUND");

    public static ErrorDetails AlreadyDeleted => 
        new("Comment already deleted", "COMMENT_ALREADY_DELETED");

    public static ErrorDetails UpdateDeletedComment => 
        new("Can't update deleted comment", "UPDATE_DELETED_COMMENT");
}