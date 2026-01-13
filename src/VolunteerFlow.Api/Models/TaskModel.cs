namespace VolunteerFlow.Api.Models;

public class TaskModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal EstimatedHours { get; set; }
    public DateTime Deadline { get; set; }
    public int CreatedByAdminId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User CreatedByAdmin { get; set; } = null!;
    public ICollection<TaskAssignment> Assignments { get; set; } = new List<TaskAssignment>();
}
