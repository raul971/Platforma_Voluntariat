namespace VolunteerFlow.Api.DTOs.Meetings;

public class MeetingCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string LocationOrLink { get; set; } = string.Empty;
}
