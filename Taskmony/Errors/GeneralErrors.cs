using System.Net;

namespace Taskmony.Errors;

public static class GeneralErrors
{
    public static ErrorDetails InternalServerError =>
        new("Internal server error", "INTERNAL_SERVER_ERROR", (int)HttpStatusCode.InternalServerError);
}