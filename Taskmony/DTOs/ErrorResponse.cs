using Taskmony.Errors;

namespace Taskmony.DTOs;

public record ErrorResponse(IReadOnlyCollection<ErrorDetails> Errors);