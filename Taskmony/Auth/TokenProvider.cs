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

    public async Task<(string accessToken, string refreshToken)> RefreshTokensAsync(string accessToken, string refreshToken)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        try
        {
            jwtSecurityTokenHandler.ValidateToken(accessToken, _tokenValidationParameters, out var validatedToken);

            if (validatedToken is JwtSecurityToken jwtSecurityToken && !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256, StringComparison.InvariantCulture))
            {
                throw new DomainException(TokenErrors.InvalidToken);
            }

            return await CreateNewTokensAsync(accessToken, refreshToken);
        }
        catch (SecurityTokenExpiredException)
        {
            return await CreateNewTokensAsync(accessToken, refreshToken);
        }
        catch (DomainException)
        {
            throw;
        }
        catch
        {
            throw new DomainException(TokenErrors.InvalidToken);
        }
    }

    private async Task<(string accessToken, string refreshToken)> CreateNewTokensAsync(string accessToken, string refreshToken)
    {
        var (claims, decodedJwt) = DecodeExpiredJwt(accessToken);
        var storedRefreshToken = await _refreshTokenRepository.GetAsync(refreshToken);

        if (storedRefreshToken is null || !Guid.TryParse(claims.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            || storedRefreshToken.UserId != userId)
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

        var user = await _userRepository.GetByIdAsync(storedRefreshToken.UserId);

        if (user is null)
        {
            throw new DomainException(TokenErrors.InvalidToken);
        }

        storedRefreshToken.IsUsed = true;

        await _refreshTokenRepository.SaveChangesAsync();

        return await GenerateTokensAsync(user);
    }

    public (ClaimsPrincipal, JwtSecurityToken) DecodeExpiredJwt(string token)
    {
        var principal = new JwtSecurityTokenHandler()
            .ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuer = _tokenValidationParameters.ValidateIssuer,
                    ValidIssuer = _tokenValidationParameters.ValidIssuer,
                    ValidateIssuerSigningKey = _tokenValidationParameters.ValidateIssuerSigningKey,
                    IssuerSigningKey = _tokenValidationParameters.IssuerSigningKey,
                    ValidAudience = _tokenValidationParameters.ValidAudience,
                    ValidateAudience = _tokenValidationParameters.ValidateAudience,
                    ValidateLifetime = false,
                },
                out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256, StringComparison.InvariantCulture))
        {
            throw new DomainException(TokenErrors.InvalidToken);
        }

        return (principal, jwtSecurityToken);
    }

    private JwtSecurityToken GenerateJwt(User user)
    {
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!.Value),
            new(JwtRegisteredClaimNames.Name, user.DisplayName!.Value),
            new(JwtRegisteredClaimNames.UniqueName, user.Login!.Value)
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