using Taskmony.DTOs;
using Taskmony.Models;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Services.Abstract;

public interface ISecurityService
{
    Task<UserAuthResponse> AuthenticateAsync(UserAuthRequest request);

    Task<RefreshTokenResponse> RefreshTokensAsync(RefreshTokenRequest request);

    Task<bool> RevokeTokenAsync(string refreshToken);

    Task<bool> RevokeAllUserTokensAsync();

    Task SendConfirmationEmailAsync(User user, Uri baseUri);

    Task ConfirmEmailAsync(Guid userId, Guid token);
}