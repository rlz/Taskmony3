namespace Taskmony.Errors;

public static class ValidationErrors
{
    public static ErrorDetails InvalidId => new("Invalid id format", "INVALID_ID");

    public static ErrorDetails InvalidOffset => new("Offset must not be negative", "INVALID_OFFSET");

    public static ErrorDetails InvalidLimit => new("Limit must not be negative", "INVALID_LIMIT");

    public static ErrorDetails InvalidDescription =>
        new("Description must not be empty", "INVALID_DESCRIPTION");

    public static ErrorDetails InvalidDeletedAt =>
        new("Deletion date must not be in the future", "INVALID_DELETED_AT");

    public static ErrorDetails InvalidCompletedAt =>
        new("Completion date must not be in the future", "INVALID_COMPLETED_AT");

    public static ErrorDetails InvalidReviewedAt =>
        new("Review date must not be in the future", "INVALID_REVIEWED_AT");

    public static ErrorDetails InvalidCommentText =>
        new("Comment text must not be empty", "INVALID_COMMENT_TEXT");

    public static ErrorDetails InvalidNotificationReadTime => new(
        "Notification read time must not be in the future",
        "INVALID_NOTIFICATION_READ_TIME");

    public static ErrorDetails InvalidDirectionName => 
        new("Direction name must not be empty", "INVALID_DIRECTION_NAME");
    
    public static ErrorDetails InvalidRepeatMode => 
        new("Repeat mode is invalid", "INVALID_REPEAT_MODE");
}