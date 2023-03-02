namespace Taskmony.Errors;

public static class TokenErrors
{
    public static ErrorDetails InvalidToken => new("Invalid  token", "INVALID_TOKEN");

    public static ErrorDetails RefreshTokenExpired => new("Refresh token expired", "REFRESH_TOKEN_EXPIRED");

    public static ErrorDetails AccessTokenNotExpired => 
        new("Acces token has not yet expired", "ACCESS_TOKEN_NOT_EXPIRED");

    public static ErrorDetails RefreshTokenAlreadyUsed => 
        new("Refresh token has already been used", "REFRESH_TOKEN_ALREADY_USED");

    public static ErrorDetails RefreshTokenRevoked => 
        new("Refresh token has been revoked", "REFRESH_TOKEN_REVOKED");
}