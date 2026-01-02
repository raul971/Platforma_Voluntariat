namespace VolunteerFlow.Api.Models;

public class MeetingInvitation
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public int VolunteerId { get; set; }
    public string Response { get; set; } = "Pending"; // Pending, Going, NotGoing
    public bool? Attended { get; set; }
    public DateTime? AttendanceMarkedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Meeting Meeting { get; set; } = null!;
    public User Volunteer { get; set; } = null!;
}
