using VolunteerFlow.Api.DTOs.Users;

namespace VolunteerFlow.Api.DTOs.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserReadDto User { get; set; } = null!;
}
