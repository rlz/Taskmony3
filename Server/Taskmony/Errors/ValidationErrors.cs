namespace Taskmony.Errors;

public static class ValidationErrors
{
    public static ErrorDetails InvalidId => new("Invalid id format", "INVALID_ID");

    public static ErrorDetails InvalidPassword => new(
        "Password must contain minimum eight characters, at least one uppercase letter, one lowercase letter, one number and optionally special characters",
        "INVALID_PASSWORD");

    public static ErrorDetails InvalidLogin => new(
        "Login must contain minimum four, maximum fifty characters and only letters and digits",
        "INVALID_LOGIN");

    public static ErrorDetails InvalidEmail =>
        new("Invalid email format", "INVALID_EMAIL");

    public static ErrorDetails InvalidDisplayName => new(
        "Display name must contain minimum three and maximum fifty characters",
        "INVALID_DISPLAY_NAME");

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

    public static ErrorDetails TaskIdOrGroupIdIsRequired =>
        new("Either task id or group id must be specified", "TASK_ID_OR_GROUP_ID_IS_REQUIRED");

    public static ErrorDetails EndBeforeStart =>
        new("End date must not be before start date", "END_BEFORE_START");

    public static ErrorDetails InvalidDateTimeFormat =>
        new("Invalid date time format", "INVALID_DATE_TIME_FORMAT");

    public static ErrorDetails InvalidRepeatUntil =>
        new("Repeat until must not be in the past", "INVALID_REPEAT_UNTIL");

    public static ErrorDetails RepeatUntilIsRequired =>
        new("Repeat until must be specified", "REPEAT_UNTIL_IS_REQUIRED");

    public static ErrorDetails RepeatEveryIsRequired =>
        new("Repeat every must be specified", "REPEAT_EVERY_IS_REQUIRED");

    public static ErrorDetails RepeatModeIsRequired =>
        new("Repeat mode must be specified", "REPEAT_MODE_IS_REQUIRED");

    public static ErrorDetails WeekDaysAreRequired =>
        new("Week days must be specified if repeat mode is weekly", "WEEK_DAYS_ARE_REQUIRED");

    public static ErrorDetails RepeatUntilIsBeforeStartAt =>
        new("Repeat until date must be after start at date", "REPEAT_UNTIL_IS_BEFORE_START_AT");
}