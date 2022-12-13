using Taskmony.DTOs;

namespace Taskmony.Services;

public interface ITokenService
{
    Task<(string? error, UserAuthResponse? response)> Authenticate(UserAuthRequest request);
}