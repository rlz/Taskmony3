namespace Taskmony.Errors;

public static class ValidationErrors
{
    public static ErrorDetails InvalidId => new("Invalid id format", "INVALID_ID");
}