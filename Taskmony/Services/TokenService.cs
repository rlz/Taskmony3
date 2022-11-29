using Taskmony.DTOs;
using Taskmony.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Taskmony.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public (string?, UserAuthResponse) CreateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.DisplayName),
            new Claim(ClaimTypes.Name, user.Login)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Authentication:Schemes:Bearer:ValidIssuer"],
            audience: _config["Authentication:Schemes:Bearer:ValidAudience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Authentication:Schemes:Bearer:Key"]!)),
                SecurityAlgorithms.HmacSha256
            )
        );

        return (null, new UserAuthResponse
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
}