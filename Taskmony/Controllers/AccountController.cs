using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taskmony.DTOs;
using Taskmony.Services.Abstract;

namespace Taskmony.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ISecurityService _securityService;

    public AccountController(IUserService userService, ISecurityService securityService)
    {
        _userService = userService;
        _securityService = securityService;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] UserAuthRequest request)
    {
        return Ok(await _securityService.AuthenticateAsync(request));
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] UserRegisterRequest request)
    {
        return Ok(await _userService.AddUserAsync(request));
    }

    [HttpPost("token/refresh")]
    public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        return Ok(await _securityService.RefreshTokensAsync(request));
    }

    [Authorize]
    [HttpPost("token/revoke")]
    public async Task<ActionResult> RevokeToken([FromBody] string refreshToken)
    {
        return Ok(await _securityService.RevokeTokenAsync(refreshToken));
    }

    [Authorize]
    [HttpPost("token/revoke/all")]
    public async Task<ActionResult> RevokeToken()
    {
        return Ok(await _securityService.RevokeAllUserTokensAsync());
    }
}