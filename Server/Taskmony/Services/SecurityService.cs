using Taskmony.Auth;
using Taskmony.DTOs;
using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Repositories.Abstract;
using Taskmony.Services.Abstract;
using Taskmony.ValueObjects;

namespace Taskmony.Services;

public class SecurityService : ISecurityService
{
    private readonly ITokenProvider _tokenProvider;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public SecurityService(ITokenProvider tokenProvider, IUserRepository userRepository,
        IPasswordHasher passwordHasher, IRefreshTokenRepository refreshTokenRepository,
        IUserIdentifierProvider userIdentifierProvider)
    {
        _tokenProvider = tokenProvider;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _refreshTokenRepository = refreshTokenRepository;
        _userIdentifierProvider = userIdentifierProvider;
    }

    public async Task<UserAuthResponse> AuthenticateAsync(UserAuthRequest request)
    {
        var login = Login.From(request.Login);
        var password = Password.From(request.Password);

        var user = await _userRepository.GetByLoginAsync(login);

        if (user is null || !_passwordHasher.VerifyPassword(password.Value, user.Password!))
        {
            throw new DomainException(UserErrors.WrongLoginOrPassword);
        }

        var (accessToken, refreshToken) = await _tokenProvider.GenerateTokensAsync(user);

        return new UserAuthResponse(user.Id, user.DisplayName!.Value, accessToken, refreshToken);
    }

    public async Task<RefreshTokenResponse> RefreshTokensAsync(RefreshTokenRequest request)
    {
        var (accessToken, refreshToken) = await _tokenProvider.RefreshTokensAsync(request.RefreshToken);

        return new RefreshTokenResponse(accessToken, refreshToken);
    }

    public async Task<bool> RevokeAllUserTokensAsync()
    {
        return await _tokenProvider.RevokeUserRefreshTokens(_userIdentifierProvider.UserId);
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var token = await _refreshTokenRepository.GetAsync(refreshToken);

        if (token is null)
        {
            throw new DomainException(TokenErrors.InvalidToken);
        }

        token.IsRevoked = true;

        return await _refreshTokenRepository.SaveChangesAsync();
    }
}