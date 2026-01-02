using VolunteerFlow.Api.DTOs.Tasks;
using VolunteerFlow.Api.DTOs.Users;

namespace VolunteerFlow.Api.DTOs.Assignments;

public class AssignmentReadDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public TaskReadDto? Task { get; set; }
    public int VolunteerId { get; set; }
    public UserReadDto? Volunteer { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? DeclineReason { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal? WorkedHours { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
