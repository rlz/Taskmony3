namespace Taskmony.Errors;

public static class GeneralErrors
{
    public static ErrorDetails InternalServerError =>
        new("Internal server error", "INTERNAL_SERVER_ERROR");

    public static ErrorDetails Forbidden =>
        new("User does not have access to this resource", "FORBIDDEN");
}