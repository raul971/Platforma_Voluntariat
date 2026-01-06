using VolunteerFlow.Api.DTOs.Users;

namespace VolunteerFlow.Api.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserReadDto>> GetAllVolunteersAsync();
    Task<UserReadDto?> GetByIdAsync(int id);
    Task<UserReadDto> CreateVolunteerAsync(UserCreateDto dto);
    Task<UserReadDto?> UpdateAsync(int id, UserUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
