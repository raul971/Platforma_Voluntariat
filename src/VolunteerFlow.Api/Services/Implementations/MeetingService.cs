using Microsoft.EntityFrameworkCore;
using VolunteerFlow.Api.Data;
using VolunteerFlow.Api.DTOs.Meetings;
using VolunteerFlow.Api.DTOs.MeetingInvitations;
using VolunteerFlow.Api.DTOs.Users;
using VolunteerFlow.Api.Models;
using VolunteerFlow.Api.Services.Interfaces;

namespace VolunteerFlow.Api.Services.Implementations;

public class MeetingService : IMeetingService
{
    private readonly ApplicationDbContext _context;

    public MeetingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MeetingReadDto> CreateMeetingAsync(int adminId, MeetingCreateDto dto)
    {
        // Verify admin exists and is Admin
        var admin = await _context.Users.FindAsync(adminId);
        if (admin == null || admin.Role != "Admin")
        {
            throw new UnauthorizedAccessException("Only admins can create meetings");
        }

        // Create meeting
        var meeting = new Meeting
        {
            Title = dto.Title,
            Description = dto.Description,
            StartAt = dto.StartAt,
            EndAt = dto.EndAt,
            LocationOrLink = dto.LocationOrLink,
            CreatedByAdminId = adminId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Meetings.Add(meeting);
        await _context.SaveChangesAsync();

        // Load admin for response
        await _context.Entry(meeting).Reference(m => m.CreatedByAdmin).LoadAsync();

        return MapToReadDto(meeting);
    }

    public async Task<MeetingReadDto?> GetMeetingByIdAsync(int meetingId)
    {
        var meeting = await _context.Meetings
            .Include(m => m.CreatedByAdmin)
            .FirstOrDefaultAsync(m => m.Id == meetingId);

        return meeting == null ? null : MapToReadDto(meeting);
    }

    public async Task<List<MeetingReadDto>> GetAllMeetingsAsync()
    {
        var meetings = await _context.Meetings
            .Include(m => m.CreatedByAdmin)
            .OrderByDescending(m => m.StartAt)
            .ToListAsync();

        return meetings.Select(MapToReadDto).ToList();
    }

    public async Task InviteVolunteersAsync(int meetingId, int adminId, MeetingInviteDto dto)
    {
        // Verify meeting exists
        var meeting = await _context.Meetings.FindAsync(meetingId);
        if (meeting == null)
        {
            throw new ArgumentException("Meeting not found");
        }

        // Verify admin
        var admin = await _context.Users.FindAsync(adminId);
        if (admin == null || admin.Role != "Admin")
        {
            throw new UnauthorizedAccessException("Only admins can invite volunteers");
        }

        // Create invitations
        foreach (var volunteerId in dto.VolunteerIds)
        {
            // Check if volunteer exists
            var volunteer = await _context.Users.FindAsync(volunteerId);
            if (volunteer == null || volunteer.Role != "Volunteer")
            {
                continue; // Skip invalid volunteers
            }

            // Check if invitation already exists
            var existingInvitation = await _context.MeetingInvitations
                .FirstOrDefaultAsync(mi => mi.MeetingId == meetingId && mi.VolunteerId == volunteerId);

            if (existingInvitation == null)
            {
                var invitation = new MeetingInvitation
                {
                    MeetingId = meetingId,
                    VolunteerId = volunteerId,
                    Response = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                _context.MeetingInvitations.Add(invitation);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task RespondToInvitationAsync(int invitationId, int volunteerId, InvitationRespondDto dto)
    {
        var invitation = await _context.MeetingInvitations
            .FirstOrDefaultAsync(mi => mi.Id == invitationId);

        if (invitation == null)
        {
            throw new ArgumentException("Invitation not found");
        }

        // Verify it's the correct volunteer
        if (invitation.VolunteerId != volunteerId)
        {
            throw new UnauthorizedAccessException("You can only respond to your own invitations");
        }

        // Update response
        if (dto.Response != "Going" && dto.Response != "NotGoing")
        {
            throw new ArgumentException("Response must be 'Going' or 'NotGoing'");
        }

        invitation.Response = dto.Response;
        await _context.SaveChangesAsync();
    }

    public async Task MarkAttendanceAsync(int invitationId, int adminId, AttendanceDto dto)
    {
        // Verify admin
        var admin = await _context.Users.FindAsync(adminId);
        if (admin == null || admin.Role != "Admin")
        {
            throw new UnauthorizedAccessException("Only admins can mark attendance");
        }

        var invitation = await _context.MeetingInvitations
            .FirstOrDefaultAsync(mi => mi.Id == invitationId);

        if (invitation == null)
        {
            throw new ArgumentException("Invitation not found");
        }

        invitation.Attended = dto.Attended;
        invitation.AttendanceMarkedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<List<InvitationReadDto>> GetMeetingInvitationsAsync(int meetingId)
    {
        var invitations = await _context.MeetingInvitations
            .Include(mi => mi.Meeting)
                .ThenInclude(m => m.CreatedByAdmin)
            .Include(mi => mi.Volunteer)
            .Where(mi => mi.MeetingId == meetingId)
            .ToListAsync();

        return invitations.Select(MapInvitationToReadDto).ToList();
    }

    public async Task<List<InvitationReadDto>> GetVolunteerInvitationsAsync(int volunteerId)
    {
        var invitations = await _context.MeetingInvitations
            .Include(mi => mi.Meeting)
                .ThenInclude(m => m.CreatedByAdmin)
            .Include(mi => mi.Volunteer)
            .Where(mi => mi.VolunteerId == volunteerId)
            .OrderByDescending(mi => mi.Meeting.StartAt)
            .ToListAsync();

        return invitations.Select(MapInvitationToReadDto).ToList();
    }

    private MeetingReadDto MapToReadDto(Meeting meeting)
    {
        return new MeetingReadDto
        {
            Id = meeting.Id,
            Title = meeting.Title,
            Description = meeting.Description,
            StartAt = meeting.StartAt,
            EndAt = meeting.EndAt,
            LocationOrLink = meeting.LocationOrLink,
            CreatedByAdminId = meeting.CreatedByAdminId,
            CreatedByAdmin = meeting.CreatedByAdmin == null ? null : new UserReadDto
            {
                Id = meeting.CreatedByAdmin.Id,
                Email = meeting.CreatedByAdmin.Email,
                FullName = meeting.CreatedByAdmin.FullName,
                Role = meeting.CreatedByAdmin.Role,
                CreatedAt = meeting.CreatedByAdmin.CreatedAt
            },
            CreatedAt = meeting.CreatedAt
        };
    }

    private InvitationReadDto MapInvitationToReadDto(MeetingInvitation invitation)
    {
        return new InvitationReadDto
        {
            Id = invitation.Id,
            MeetingId = invitation.MeetingId,
            Meeting = invitation.Meeting == null ? null : MapToReadDto(invitation.Meeting),
            VolunteerId = invitation.VolunteerId,
            Volunteer = invitation.Volunteer == null ? null : new UserReadDto
            {
                Id = invitation.Volunteer.Id,
                Email = invitation.Volunteer.Email,
                FullName = invitation.Volunteer.FullName,
                Role = invitation.Volunteer.Role,
                CreatedAt = invitation.Volunteer.CreatedAt
            },
            Response = invitation.Response,
            Attended = invitation.Attended,
            AttendanceMarkedAt = invitation.AttendanceMarkedAt,
            CreatedAt = invitation.CreatedAt
        };
    }
}
