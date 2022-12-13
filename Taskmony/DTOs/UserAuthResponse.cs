namespace Taskmony.DTOs;

public record UserAuthResponse(Guid Id, string DisplayName, string AccessToken);