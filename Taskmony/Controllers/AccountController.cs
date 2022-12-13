using Microsoft.AspNetCore.Mvc;
using Taskmony.DTOs;
using Taskmony.Services;

namespace Taskmony.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public AccountController(IUserService userService, ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] UserAuthRequest request)
    {
        var result = await _tokenService.Authenticate(request);

        if (result.error is not null)
        {
            return BadRequest(result.error);
        }

        return Ok(result.response);
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] UserRegisterRequest request)
    {
        var error = await _userService.AddUserAsync(request);

        if (error is not null)
        {
            return BadRequest(error);
        }

        return Ok();
    }
}