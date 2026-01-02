using VolunteerFlow.Api.DTOs.Meetings;
using VolunteerFlow.Api.DTOs.Users;

namespace VolunteerFlow.Api.DTOs.MeetingInvitations;

public class InvitationReadDto
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public MeetingReadDto? Meeting { get; set; }
    public int VolunteerId { get; set; }
    public UserReadDto? Volunteer { get; set; }
    public string Response { get; set; } = string.Empty;
    public bool? Attended { get; set; }
    public DateTime? AttendanceMarkedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
