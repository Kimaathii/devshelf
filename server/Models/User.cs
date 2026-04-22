namespace server.Models;
public class User
{
    public Guid Id { get; set; }
    public required string UserName { get; set;}
    public required string Email { get; set; }
    public string PasswordHash {  get; private set; } = string.Empty;

    public void SetPasswordHash(string hash)
    {
        PasswordHash = hash;
    }

    public DateTime CreatedAt { get; set; }
}