using VolunteerFlow.Api.DTOs.Meetings;
using VolunteerFlow.Api.DTOs.MeetingInvitations;

namespace VolunteerFlow.Api.Services.Interfaces;

public interface IMeetingService
{
    Task<MeetingReadDto> CreateMeetingAsync(int adminId, MeetingCreateDto dto);
    Task<MeetingReadDto?> GetMeetingByIdAsync(int meetingId);
    Task<List<MeetingReadDto>> GetAllMeetingsAsync();
    Task InviteVolunteersAsync(int meetingId, int adminId, MeetingInviteDto dto);
    Task RespondToInvitationAsync(int invitationId, int volunteerId, InvitationRespondDto dto);
    Task MarkAttendanceAsync(int invitationId, int adminId, AttendanceDto dto);
    Task<List<InvitationReadDto>> GetMeetingInvitationsAsync(int meetingId);
    Task<List<InvitationReadDto>> GetVolunteerInvitationsAsync(int volunteerId);
}
