namespace VolunteerFlow.Api.DTOs.Users;

public class UserUpdateDto
{
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? Password { get; set; } // Optional: only if changing password
}
