using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;  // For [Authorize]
using System.Security.Claims;                // For ClaimTypes
using server.Services;
using server.DTOs.Auth;

namespace server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{

[HttpGet]
public IActionResult GetProfile()
    {
        // 1. Extract claims from User
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;

        // 2. Check for null values
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userName))
            return Unauthorized(new { error = "Invalid token: missing required claims" });

        // 3. Convert userId string to Guid
        var id = Guid.Parse(userId);

        // 4. Create and return ProfileResponseDto
        return Ok(new ProfileResponseDto
        {
            Id = id,
            Email = email,
            UserName = userName,
            FetchedAt = DateTime.UtcNow
        });
    }
}


