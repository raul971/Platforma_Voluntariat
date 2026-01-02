namespace VolunteerFlow.Api.DTOs.Reports;

public class VolunteerHoursReportDto
{
    public int VolunteerId { get; set; }
    public string VolunteerName { get; set; } = string.Empty;
    public decimal TotalHours { get; set; }
    public List<HourDetailDto> Details { get; set; } = new();
}
