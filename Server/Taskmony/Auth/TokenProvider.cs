using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models;
using Taskmony.Repositories.Abstract;

namespace Taskmony.Auth;

public class TokenProvider : ITokenProvider
{
    private readonly JwtOptions _options;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public TokenProvider(IOptions<JwtOptions> options, IRefreshTokenRepository refreshTokenRepository,
        IOptionsMonitor<JwtBearerOptions> jwtOptions, IUserRepository userRepository)
    {
        _options = options.Value;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenValidationParameters = jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme).TokenValidationParameters;
        _userRepository = userRepository;
    }

    public async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User user)
    {
        var jwt = GenerateJwt(user);
        var token = GenerateToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_options.RefreshTokenExpirationMinutes)
        };

        // TODO: encrypt refresh token

        await _refreshTokenRepository.AddAsync(refreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return (new JwtSecurityTokenHandler().WriteToken(jwt), token);
    }

    public async Task<(string accessToken, string refreshToken)> RefreshTokensAsync(string refreshToken)
    {
        var storedRefreshToken = await _refreshTokenRepository.GetAsync(refreshToken);

        if (storedRefreshToken is null)
        {
            throw new DomainException(TokenErrors.InvalidToken);
        }

        if (storedRefreshToken.IsUsed)
        {
            throw new DomainException(TokenErrors.RefreshTokenAlreadyUsed);
        }

        if (storedRefreshToken.IsRevoked)
        {
            throw new DomainException(TokenErrors.RefreshTokenRevoked);
        }

        if (storedRefreshToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new DomainException(TokenErrors.RefreshTokenExpired);
        }

        var user = await _userRepository.GetByIdAsync(storedRefreshToken.UserId);

        if (user is null)
        {
            throw new DomainException(TokenErrors.InvalidToken);
        }

        storedRefreshToken.IsUsed = true;

        await _refreshTokenRepository.SaveChangesAsync();

        return await GenerateTokensAsync(user);
    }

    public async Task<bool> RevokeUserRefreshTokens(Guid userId)
    {
        var refreshTokens = await _refreshTokenRepository.GetByUserIdAsync(userId);

        refreshTokens.ToList().ForEach(token => token.IsRevoked = true);

        return await _refreshTokenRepository.SaveChangesAsync();
    }

    private async Task<(string accessToken, string refreshToken)> CreateNewTokensAsync(string accessToken, string refreshToken)
    {
        var storedRefreshToken = await _refreshTokenRepository.GetAsync(refreshToken);

        if (storedRefreshToken is null)
        {
            throw new DomainException(TokenErrors.InvalidToken);
        }

        if (storedRefreshToken.IsUsed)
        {
            throw new DomainException(TokenErrors.RefreshTokenAlreadyUsed);
        }

        if (storedRefreshToken.IsRevoked)
        {
            throw new DomainException(TokenErrors.RefreshTokenRevoked);
        }

        if (storedRefreshToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new DomainException(TokenErrors.RefreshTokenExpired);
        }

        var user = await _userRepository.GetByIdAsync(storedRefreshToken.UserId);

        if (user is null)
        {
            throw new DomainException(TokenErrors.InvalidToken);
        }

        storedRefreshToken.IsUsed = true;

        await _refreshTokenRepository.SaveChangesAsync();

        return await GenerateTokensAsync(user);
    }

    private JwtSecurityToken GenerateJwt(User user)
    {
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret)),
                SecurityAlgorithms.HmacSha256
            )
        );

        return token;
    }

    private string GenerateToken()
    {
        var randomNumber = new byte[32];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}