using VolunteerFlow.Api.DTOs.Auth;

namespace VolunteerFlow.Api.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
}
