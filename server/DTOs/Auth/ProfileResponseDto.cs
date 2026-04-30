namespace server.DTOs.Auth;

public class ProfileResponseDto
{
    public Guid Id { get; set; }                     // User ID (needed for future API calls)
    public required string UserName { get; set; }
    public required string Email { get; set; }

    public DateTime FetchedAt { get; set; }  // When was this profile fetched?
}