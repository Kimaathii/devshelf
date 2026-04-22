namespace server.Services;

using server.Models;
using server.DTOs.Auth;
using server.Data;
using Microsoft.EntityFrameworkCore;

public class AuthService
{
    private readonly AppDbContext _context;
    // Constructor? (might need AppDbContext)
    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    // Register method
    // Logic: hash password + save user + return result
    public async Task<(bool success, string message)> Register(RegisterDto dto)
    {
        var existingUser = await _context.Users
        .FirstOrDefaultAsync(u => u.Email == dto.Email);

        // 1. Validate input (is email already taken?)
        if (existingUser != null)
        {
            return (false, "Email already taken");
        }
        // 2. Hash the plain password using BCrypt
        string hashedPassword  = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        // 3. Create a new User object
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow
        };

        newUser.SetPasswordHash(hashedPassword);
        // 4. Save to database
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
        // 5. Return success or failure

        return (true, "User registered successfully");
    }

    // Login method  
    // Logic: find user + verify password + return result

    // Helper: hash password (use BCrypt)
    // Helper: verify password (use BCrypt)

};