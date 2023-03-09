using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taskmony.DTOs;
using Taskmony.Emails;
using Taskmony.Services.Abstract;

namespace Taskmony.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ISecurityService _securityService;
    private readonly IEmailService _emailService;

    public AccountController(IUserService userService, ISecurityService securityService,
        IEmailService emailService)
    {
        _userService = userService;
        _securityService = securityService;
        _emailService = emailService;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] UserAuthRequest request)
    {
        return Ok(await _securityService.AuthenticateAsync(request));
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] UserRegisterRequest request)
    {
        var user = await _userService.AddUserAsync(request);

        var baseUri = new Uri($"{Request.Scheme}://{Request.Host}{Request.PathBase}");

        await _securityService.SendConfirmationEmailAsync(user, baseUri);

        return Ok();
    }

    [HttpPost("token/refresh")]
    public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        return Ok(await _securityService.RefreshTokensAsync(request));
    }

    [Authorize]
    [HttpPost("token/revoke")]
    public async Task<ActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
    {
        return Ok(await _securityService.RevokeTokenAsync(request.RefreshToken));
    }

    [Authorize]
    [HttpPost("token/revoke/all")]
    public async Task<ActionResult> RevokeToken()
    {
        return Ok(await _securityService.RevokeAllUserTokensAsync());
    }

    [AllowAnonymous]
    [HttpGet("confirm-email")]
    public async Task<ActionResult> ConfirmEmail(Guid userId, Guid token)
    {
        var redirectTo = await _securityService.ConfirmEmailAsync(userId, token);

        if (redirectTo is not null)
        {
            return Redirect(redirectTo);
        }

        return Ok();
    }
}