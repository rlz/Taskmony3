namespace Taskmony.DTOs;

public record UserAuthResponse(Guid UserId, string DisplayName, string AccessToken, string RefreshToken);