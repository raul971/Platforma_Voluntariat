using VolunteerFlow.Api.DTOs.Events;
using VolunteerFlow.Api.DTOs.EventParticipations;

namespace VolunteerFlow.Api.Services.Interfaces;

public interface IEventService
{
    Task<EventReadDto> CreateEventAsync(int adminId, EventCreateDto dto);
    Task<EventReadDto?> GetEventByIdAsync(int eventId);
    Task<List<EventReadDto>> GetAllEventsAsync();
    Task InviteVolunteersAsync(int eventId, int adminId, EventInviteDto dto);
    Task RespondToParticipationAsync(int participationId, int volunteerId, ParticipationRespondDto dto);
    Task ReportOccurrenceAsync(int participationId, int volunteerId, OccurrenceReportDto dto);
    Task<List<ParticipationReadDto>> GetEventParticipationsAsync(int eventId);
    Task<List<ParticipationReadDto>> GetVolunteerParticipationsAsync(int volunteerId);
}
