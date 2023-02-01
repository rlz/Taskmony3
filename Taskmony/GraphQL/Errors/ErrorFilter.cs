using Taskmony.Errors;
using Taskmony.Exceptions;

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

        if (error.Exception is DomainException domainException)
        {
            return ErrorBuilder
                .New()
                .SetCode(domainException.Error.Code)
                .SetMessage(domainException.Error.Message)
                .SetPath(error.Path)
                .Build();
        }

        if (error.Exception is null || _environment.IsDevelopment())
        {
            return error;
        }

        return ErrorBuilder
            .New()
            .SetCode(GeneralErrors.InternalServerError.Code)
            .SetMessage(GeneralErrors.InternalServerError.Message)
            .Build();
    }
}