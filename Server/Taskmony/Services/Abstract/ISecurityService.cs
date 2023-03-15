using Taskmony.DTOs;

namespace Taskmony.Services.Abstract;

public interface ISecurityService
{
    Task<UserAuthResponse> AuthenticateAsync(UserAuthRequest request);

    Task<RefreshTokenResponse> RefreshTokensAsync(RefreshTokenRequest request);

    Task<bool> RevokeTokenAsync(string refreshToken);

    Task<bool> RevokeAllUserTokensAsync();
}