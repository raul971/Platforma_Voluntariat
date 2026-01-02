using VolunteerFlow.Api.DTOs.Users;

namespace VolunteerFlow.Api.DTOs.Events;

public class EventReadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string Location { get; set; } = string.Empty;
    public int CreatedByAdminId { get; set; }
    public UserReadDto? CreatedByAdmin { get; set; }
    public DateTime CreatedAt { get; set; }
}
