namespace Taskmony.DTOs;

public record RefreshTokenRequest(string AccessToken, string RefreshToken);