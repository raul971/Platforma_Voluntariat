using VolunteerFlow.Api.DTOs.Events;
using VolunteerFlow.Api.DTOs.Users;

namespace VolunteerFlow.Api.DTOs.EventParticipations;

public class ParticipationReadDto
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public EventReadDto? Event { get; set; }
    public int VolunteerId { get; set; }
    public UserReadDto? Volunteer { get; set; }
    public string Response { get; set; } = string.Empty;
    public string OccurrenceReport { get; set; } = string.Empty;
    public string? OccurrenceNotes { get; set; }
    public DateTime CreatedAt { get; set; }
}
