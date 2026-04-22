namespace server.DTOs.Auth;

public class AuthResponseDto
{
    public Guid Id { get; set; }                     // User ID (needed for future API calls)
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Token { get; set; }       // JWT token (make it required, not empty)
}