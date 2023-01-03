using Taskmony.Exceptions;
using Taskmony.Errors;

namespace Taskmony.GraphQL.Errors;

public class ErrorFilter : IErrorFilter
{
    private readonly ILogger _logger;
    private readonly IWebHostEnvironment _environment;

    public ErrorFilter(ILogger<ErrorFilter> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public IError OnError(IError error)
    {
        _logger.LogError(error.Exception, "An exception occurred: {Message}", error.Exception?.Message);

        if (error.Exception is DomainException ex)
        {
            return ErrorBuilder
                .New()
                .SetCode(ex.Error.Code)
                .SetMessage(ex.Error.Message)
                .Build();
        }

        if (_environment.IsDevelopment())
            return error;

        return ErrorBuilder
            .New()
            .SetCode(GeneralErrors.InternalServerError.Code)
            .SetMessage(GeneralErrors.InternalServerError.Message)
            .SetPath(error.Path)
            .Build();
    }
}