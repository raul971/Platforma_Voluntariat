namespace VolunteerFlow.Api.DTOs.Tasks;

public class TaskCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal EstimatedHours { get; set; }
    public DateTime Deadline { get; set; }
}
