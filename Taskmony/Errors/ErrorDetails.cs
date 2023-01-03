using System.Net;
using System.Text.Json;

namespace Taskmony.Errors;

public class ErrorDetails
{
    public string Message { get; }

    public string Code { get; }

    public int Status { get; }

    public ErrorDetails(string message, string code, int status = (int)HttpStatusCode.BadRequest)
    {
        Message = message;
        Code = code;
        Status = status;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}