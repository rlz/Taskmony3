using Microsoft.AspNetCore.Mvc;
using Taskmony.DTOs;
using Taskmony.Services;

namespace Taskmony.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] UserAuthRequest request)
    {
        var response = await _userService.AuthenticateUserAsync(request);

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] UserRegisterRequest request)
    {
        if (await _userService.AddUserAsync(request))
        {
            return Ok();
        }

        return BadRequest();
    }
}