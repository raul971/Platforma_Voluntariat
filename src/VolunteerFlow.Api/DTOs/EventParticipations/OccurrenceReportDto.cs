namespace VolunteerFlow.Api.DTOs.EventParticipations;

public class OccurrenceReportDto
{
    public string OccurrenceReport { get; set; } = string.Empty; // "Happened" or "DidNotHappen"
    public string? OccurrenceNotes { get; set; }
}
