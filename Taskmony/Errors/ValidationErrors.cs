namespace Taskmony.Errors;

public static class ValidationErrors
{
    public static ErrorDetails InvalidId => new("Invalid id format", "INVALID_ID");

    public static ErrorDetails InvalidOffset => new("Offset must not be negative", "INVALID_OFFSET");

    public static ErrorDetails InvalidLimit => new("Limit must not be negative", "INVALID_LIMIT");
}