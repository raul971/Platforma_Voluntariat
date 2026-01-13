namespace VolunteerFlow.Api.DTOs.Tasks;

public class TaskUpdateDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? EstimatedHours { get; set; }
    public DateTime? Deadline { get; set; }
}
