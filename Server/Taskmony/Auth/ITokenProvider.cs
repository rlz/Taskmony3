using Taskmony.Models.Users;

namespace Taskmony.Auth;

public interface ITokenProvider
{
    Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User user);

    Task<(string accessToken, string refreshToken)> RefreshTokensAsync(string refreshToken);

    Task RevokeUserRefreshTokens(Guid userId);
}