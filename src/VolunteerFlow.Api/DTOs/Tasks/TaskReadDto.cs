using VolunteerFlow.Api.DTOs.Users;

namespace VolunteerFlow.Api.DTOs.Tasks;

public class TaskReadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal EstimatedHours { get; set; }
    public DateTime Deadline { get; set; }
    public int CreatedByAdminId { get; set; }
    public UserReadDto? CreatedByAdmin { get; set; } // Optional: for expanded response
    public DateTime CreatedAt { get; set; }
}
