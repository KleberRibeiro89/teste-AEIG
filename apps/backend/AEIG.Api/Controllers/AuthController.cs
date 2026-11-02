using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AEIG.Domain.AppService.Dtos.Requests.UserRequests;
using AEIG.Domain.AppService.Dtos.Responses.UserResponses;
using AEIG.Domain.AppService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEIG.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var result = await _userService.LoginAsync(request);
        return result is null ? Unauthorized() : Ok(result);
    }

    [HttpGet("me")]
    public IActionResult Me()
    {
        var name = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(JwtRegisteredClaimNames.Name);
        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(JwtRegisteredClaimNames.Email);
        return Ok(new { Name = name, Email = email });
    }
}
