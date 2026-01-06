using Microsoft.EntityFrameworkCore;
using VolunteerFlow.Api.Data;
using VolunteerFlow.Api.DTOs.Auth;
using VolunteerFlow.Api.DTOs.Users;
using VolunteerFlow.Api.Helpers;
using VolunteerFlow.Api.Services.Interfaces;

namespace VolunteerFlow.Api.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtHelper _jwtHelper;

    public AuthService(ApplicationDbContext context, JwtHelper jwtHelper)
    {
        _context = context;
        _jwtHelper = jwtHelper;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        // Validate input
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            throw new ArgumentException("Email and password are required");
        }

        // Find user by email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        // Check if user exists and password is correct
        if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Generate JWT token
        var token = _jwtHelper.GenerateToken(user.Id, user.Email, user.Role);

        // Map to UserReadDto
        var userDto = new UserReadDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };

        // Return LoginResponseDto
        return new LoginResponseDto
        {
            Token = token,
            User = userDto
        };
    }
}
