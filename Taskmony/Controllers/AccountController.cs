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
        return Ok(await _userService.AddUserAsync(request));
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
    [HttpPost("confirm-email")]
    public async Task<ActionResult> SendConfirmationEmail(string emailTo)
    {
        // TODO: implement email confirmation
        
        await _emailService.SendEmailAsync(emailTo, "Kek", "Hello");

        return Ok();
    }
}