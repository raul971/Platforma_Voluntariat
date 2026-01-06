using Microsoft.EntityFrameworkCore;
using VolunteerFlow.Api.Data;
using VolunteerFlow.Api.DTOs.Reports;
using VolunteerFlow.Api.Services.Interfaces;

namespace VolunteerFlow.Api.Services.Implementations;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;

    public ReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VolunteerHoursReportDto?> GetVolunteerHoursReportAsync(int volunteerId)
    {
        var volunteer = await _context.Users.FindAsync(volunteerId);

        if (volunteer == null || volunteer.Role != "Volunteer")
        {
            return null;
        }

        var details = new List<HourDetailDto>();
        decimal totalHours = 0;

        // Get hours from completed tasks
        var completedTasks = await _context.TaskAssignments
            .Include(ta => ta.Task)
            .Where(ta => ta.VolunteerId == volunteerId && ta.Status == "Completed")
            .ToListAsync();

        foreach (var assignment in completedTasks)
        {
            var hours = assignment.WorkedHours ?? 0;
            totalHours += hours;

            details.Add(new HourDetailDto
            {
                Type = "Task",
                Title = assignment.Task.Title,
                Hours = hours,
                Date = assignment.CompletedAt ?? assignment.CreatedAt
            });
        }

        // Get hours from attended meetings
        var attendedMeetings = await _context.MeetingInvitations
            .Include(mi => mi.Meeting)
            .Where(mi => mi.VolunteerId == volunteerId && mi.Attended == true)
            .ToListAsync();

        foreach (var invitation in attendedMeetings)
        {
            var duration = (decimal)(invitation.Meeting.EndAt - invitation.Meeting.StartAt).TotalHours;
            totalHours += duration;

            details.Add(new HourDetailDto
            {
                Type = "Meeting",
                Title = invitation.Meeting.Title,
                Hours = duration,
                Date = invitation.Meeting.StartAt
            });
        }

        // Get hours from happened events
        var happenedEvents = await _context.EventParticipations
            .Include(ep => ep.Event)
            .Where(ep => ep.VolunteerId == volunteerId && ep.OccurrenceReport == "Happened")
            .ToListAsync();

        foreach (var participation in happenedEvents)
        {
            var duration = (decimal)(participation.Event.EndAt - participation.Event.StartAt).TotalHours;
            totalHours += duration;

            details.Add(new HourDetailDto
            {
                Type = "Event",
                Title = participation.Event.Title,
                Hours = duration,
                Date = participation.Event.StartAt
            });
        }

        // Sort details by date (most recent first)
        details = details.OrderByDescending(d => d.Date).ToList();

        return new VolunteerHoursReportDto
        {
            VolunteerId = volunteer.Id,
            VolunteerName = volunteer.FullName,
            TotalHours = totalHours,
            Details = details
        };
    }

    public async Task<IEnumerable<VolunteerHoursReportDto>> GetAllVolunteersHoursReportAsync()
    {
        var volunteers = await _context.Users
            .Where(u => u.Role == "Volunteer")
            .ToListAsync();

        var reports = new List<VolunteerHoursReportDto>();

        foreach (var volunteer in volunteers)
        {
            var report = await GetVolunteerHoursReportAsync(volunteer.Id);

            if (report != null)
            {
                reports.Add(report);
            }
        }

        // Sort by total hours (highest first)
        return reports.OrderByDescending(r => r.TotalHours).ToList();
    }
}
