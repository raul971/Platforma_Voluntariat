namespace VolunteerFlow.Api.Models;

public class EventParticipation
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int VolunteerId { get; set; }
    public string Response { get; set; } = "Pending"; // Pending, Going, NotGoing
    public string OccurrenceReport { get; set; } = "Unknown"; // Unknown, Happened, DidNotHappen
    public string? OccurrenceNotes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Event Event { get; set; } = null!;
    public User Volunteer { get; set; } = null!;
}
