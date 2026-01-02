using VolunteerFlow.Api.DTOs.Users;

namespace VolunteerFlow.Api.DTOs.Meetings;

public class MeetingReadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string LocationOrLink { get; set; } = string.Empty;
    public int CreatedByAdminId { get; set; }
    public UserReadDto? CreatedByAdmin { get; set; }
    public DateTime CreatedAt { get; set; }
}
