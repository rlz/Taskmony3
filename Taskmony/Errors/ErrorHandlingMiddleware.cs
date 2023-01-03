using System.Net;
using System.Text.Json;
using Taskmony.DTOs;
using Taskmony.Exceptions;

namespace Taskmony.Errors;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred: {Message}", ex.Message);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        context.Response.StatusCode = exception switch
        {
            DomainException domainException => domainException.Error.Status,
            _ => (int)HttpStatusCode.InternalServerError
        };

        IReadOnlyCollection<ErrorDetails> errors = exception switch
        {
            DomainException domainException => new[] { domainException.Error },
            _ => new[] { GeneralErrors.InternalServerError }
        };

        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var response = JsonSerializer.Serialize(new ErrorResponse(errors), serializerOptions);

        await context.Response.WriteAsync(response);
    }
}