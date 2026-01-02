namespace VolunteerFlow.Api.DTOs.Reports;

public class HourDetailDto
{
    public string Type { get; set; } = string.Empty; // "Task", "Meeting", "Event"
    public string Title { get; set; } = string.Empty;
    public decimal Hours { get; set; }
    public DateTime Date { get; set; }
}
