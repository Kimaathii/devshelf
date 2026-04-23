using Microsoft.AspNetCore.Mvc;
using server.Services;
using server.DTOs.Auth;

namespace server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        // Call AuthService.Register()
        var (success, message) = await _authService.Register(dto);
        if (!success) return BadRequest(new {error = message});
        // Return response
        return Ok(new {message = "Registration successful"});
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        // Call AuthService.Login()
        var (success, message, response) = await _authService.Login(dto);
        if (!success) return BadRequest(new {error = message});
        // Return response
        return Ok(response);
    }
}