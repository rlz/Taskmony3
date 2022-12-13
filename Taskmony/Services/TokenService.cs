using Taskmony.Auth;
using Taskmony.DTOs;

namespace Taskmony.Services;

public class TokenService : ITokenService
{
    private readonly IJwtProvider _jwtProvider;
    private readonly IUserService _userService;

    public TokenService(IJwtProvider jwtProvider, IUserService userService)
    {
        _jwtProvider = jwtProvider;
        _userService = userService;
    }

    public async Task<(string? error, UserAuthResponse? response)> Authenticate(UserAuthRequest request)
    {
        var (error, user) = await _userService.GetUserAsync(request);

        if (error is not null)
        {
            return (error, null);
        }

        if (user is null)
        {
            return ("User not found", null);
        }

        var token = _jwtProvider.GenerateToken(user);

        return (null, new UserAuthResponse(user.Id, user.DisplayName!, token));
    }
}