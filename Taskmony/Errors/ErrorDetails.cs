using System.Text.Json;

namespace Taskmony.Errors;

public class ErrorDetails
{
    public string Message { get; }

    public string Code { get; }

    public ErrorDetails(string message, string code)
    {
        Message = message;
        Code = code;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}