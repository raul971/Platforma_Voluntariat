namespace VolunteerFlow.Api.DTOs.Users;

public class UserCreateDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    // Role is set to "Volunteer" by default in service
}
