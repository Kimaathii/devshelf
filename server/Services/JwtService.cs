namespace server.Services;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using server.Models;

public class JwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;

    }

    public string GenerateToken(User user)
    {
        // 1. Get secret key from appsettings
        var secretkey = _config["Jwt:Key"];
        if(secretkey == null)
            throw new Exception("JWT Key is not set");
        // 2. Create security key
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretkey));
        // 3. Create credentials
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        // 4. Create claims (userId, email, name)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),

        };
        // 5. Create JWT token
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );
        // 6. Return token string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}