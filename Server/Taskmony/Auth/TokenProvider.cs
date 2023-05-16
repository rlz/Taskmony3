using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Users;
using Taskmony.Repositories.Abstract;

namespace Taskmony.Auth;

public class TokenProvider : ITokenProvider
{
    private readonly JwtOptions _options;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;

    public TokenProvider(IOptions<JwtOptions> options, IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository)
    {
        _options = options.Value;
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
    }

    public async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User user)
    {
        var jwt = GenerateJwt(user);
        var token = GenerateToken();

        var refreshToken = new RefreshToken(
            userId: user.Id,
            token: token,
            createdAt: DateTime.UtcNow,
            expiresAt: DateTime.UtcNow.AddMinutes(_options.RefreshTokenExpirationMinutes));

        // TODO: encrypt refresh token

        await _refreshTokenRepository.AddAsync(refreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return (new JwtSecurityTokenHandler().WriteToken(jwt), token);
    }

    public async Task<(string accessToken, string refreshToken)> RefreshTokensAsync(string refreshToken)
    {
        var storedRefreshToken = await _refreshTokenRepository.GetAsync(refreshToken);
        
        if (storedRefreshToken == null)
        {
            throw new DomainException(TokenErrors.InvalidToken);
        }
        
        var user = await _userRepository.GetByIdAsync(storedRefreshToken.UserId);

        if (user == null)
        {
            throw new DomainException(TokenErrors.InvalidToken);
        }

        storedRefreshToken.Use();

        await _refreshTokenRepository.SaveChangesAsync();

        return await GenerateTokensAsync(user);
    }

    public async Task<bool> RevokeUserRefreshTokens(Guid userId)
    {
        var refreshTokens = await _refreshTokenRepository.GetByUserIdAsync(userId);

        refreshTokens.ToList().ForEach(token => token.Revoke());

        return await _refreshTokenRepository.SaveChangesAsync();
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

        using var rng = RandomNumberGenerator.Create();
        
        rng.GetBytes(randomNumber);
        
        return Convert.ToBase64String(randomNumber);
    }
}