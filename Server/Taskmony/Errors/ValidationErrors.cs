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
        "Display name must contain minimum 3 and maximum 100 characters",
        "INVALID_DISPLAY_NAME");

    public static ErrorDetails InvalidOffset => new("Offset must not be negative", "INVALID_OFFSET");

    public static ErrorDetails InvalidLimit => new("Limit must not be negative", "INVALID_LIMIT");

    public static ErrorDetails InvalidDescription =>
        new("Description must be between 1 and 500 characters", "INVALID_DESCRIPTION");

    public static ErrorDetails InvalidDeletedAt =>
        new("Deletion date must not be in the future", "INVALID_DELETED_AT");

    public static ErrorDetails InvalidCompletedAt =>
        new("Completion date must not be in the future", "INVALID_COMPLETED_AT");

    public static ErrorDetails InvalidReviewedAt =>
        new("Review date must not be in the future", "INVALID_REVIEWED_AT");

    public static ErrorDetails InvalidCommentText =>
        new("Comment text must be between 1 and 15000 characters", "INVALID_COMMENT_TEXT");

    public static ErrorDetails InvalidNotificationReadTime => new(
        "Notification read time must not be in the future",
        "INVALID_NOTIFICATION_READ_TIME");

    public static ErrorDetails InvalidDirectionName =>
        new("Direction name must be between 1 and 120 characters", "INVALID_DIRECTION_NAME");

    public static ErrorDetails InvalidRepeatMode =>
        new("Repeat mode is invalid", "INVALID_REPEAT_MODE");

    public static ErrorDetails EndBeforeStart =>
        new("End date must not be before start date", "END_BEFORE_START");

    public static ErrorDetails InvalidDateTimeFormat =>
        new("Invalid date time format", "INVALID_DATE_TIME_FORMAT");

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

    public static ErrorDetails InvalidRepeatEvery =>
        new("Repeat every must be positive", "INVALID_REPEAT_EVERY");

    public static ErrorDetails InvalidDetails =>
        new("Length of details must not be more than 16384 characters", "INVALID_DETAILS");

    public static ErrorDetails RepeatUntilIsTooFarForDailyAndWeekly => new(
        "Repeat until date must not be more than 3 years in the future for daily or weekly tasks", 
        "REPEAT_UNTIL_IS_TOO_FAR");

    public static ErrorDetails RepeatUntilIsTooFarForMonthlyAndYearly => new(
        "Repeat until date must not be more than 100 years in the future for monthly or yearly tasks", 
        "REPEAT_UNTIL_IS_TOO_FAR");
}