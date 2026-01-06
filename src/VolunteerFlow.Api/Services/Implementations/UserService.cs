using Microsoft.EntityFrameworkCore;
using VolunteerFlow.Api.Data;
using VolunteerFlow.Api.DTOs.Users;
using VolunteerFlow.Api.Helpers;
using VolunteerFlow.Api.Models;
using VolunteerFlow.Api.Services.Interfaces;

namespace VolunteerFlow.Api.Services.Implementations;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserReadDto>> GetAllVolunteersAsync()
    {
        var volunteers = await _context.Users
            .Where(u => u.Role == "Volunteer")
            .Select(u => new UserReadDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return volunteers;
    }

    public async Task<UserReadDto?> GetByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return null;
        }

        return new UserReadDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserReadDto> CreateVolunteerAsync(UserCreateDto dto)
    {
        // Validate input
        if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.FullName))
        {
            throw new ArgumentException("Email, password and full name are required");
        }

        // Check if email already exists
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"User with email {dto.Email} already exists");
        }

        // Create new volunteer
        var volunteer = new User
        {
            Email = dto.Email,
            PasswordHash = PasswordHasher.HashPassword(dto.Password),
            FullName = dto.FullName,
            Role = "Volunteer",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(volunteer);
        await _context.SaveChangesAsync();

        return new UserReadDto
        {
            Id = volunteer.Id,
            Email = volunteer.Email,
            FullName = volunteer.FullName,
            Role = volunteer.Role,
            CreatedAt = volunteer.CreatedAt
        };
    }

    public async Task<UserReadDto?> UpdateAsync(int id, UserUpdateDto dto)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return null;
        }

        // Update only provided fields
        if (!string.IsNullOrEmpty(dto.Email))
        {
            // Check if new email is already taken by another user
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Id != id);

            if (existingUser != null)
            {
                throw new InvalidOperationException($"Email {dto.Email} is already taken");
            }

            user.Email = dto.Email;
        }

        if (!string.IsNullOrEmpty(dto.FullName))
        {
            user.FullName = dto.FullName;
        }

        if (!string.IsNullOrEmpty(dto.Password))
        {
            user.PasswordHash = PasswordHasher.HashPassword(dto.Password);
        }

        await _context.SaveChangesAsync();

        return new UserReadDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return false;
        }

        // Don't allow deleting admin users
        if (user.Role == "Admin")
        {
            throw new InvalidOperationException("Cannot delete admin users");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }
}
