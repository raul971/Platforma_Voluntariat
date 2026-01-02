namespace VolunteerFlow.Api.Models;

public class TaskAssignment
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int VolunteerId { get; set; }
    public string Status { get; set; } = "Assigned"; // Assigned, Accepted, Declined, Completed
    public string? DeclineReason { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal? WorkedHours { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public TaskModel Task { get; set; } = null!;
    public User Volunteer { get; set; } = null!;
}
