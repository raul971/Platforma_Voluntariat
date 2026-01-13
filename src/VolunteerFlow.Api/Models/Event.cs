namespace VolunteerFlow.Api.Models;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string Location { get; set; } = string.Empty;
    public int CreatedByAdminId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User CreatedByAdmin { get; set; } = null!;
    public ICollection<EventParticipation> Participations { get; set; } = new List<EventParticipation>();
}
