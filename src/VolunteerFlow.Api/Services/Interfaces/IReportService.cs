using VolunteerFlow.Api.DTOs.Reports;

namespace VolunteerFlow.Api.Services.Interfaces;

public interface IReportService
{
    Task<VolunteerHoursReportDto?> GetVolunteerHoursReportAsync(int volunteerId);
    Task<IEnumerable<VolunteerHoursReportDto>> GetAllVolunteersHoursReportAsync();
}
