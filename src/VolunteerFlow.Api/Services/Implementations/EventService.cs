using Microsoft.EntityFrameworkCore;
using VolunteerFlow.Api.Data;
using VolunteerFlow.Api.DTOs.Events;
using VolunteerFlow.Api.DTOs.EventParticipations;
using VolunteerFlow.Api.DTOs.Users;
using VolunteerFlow.Api.Models;
using VolunteerFlow.Api.Services.Interfaces;

namespace VolunteerFlow.Api.Services.Implementations;

public class EventService : IEventService
{
    private readonly ApplicationDbContext _context;

    public EventService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EventReadDto> CreateEventAsync(int adminId, EventCreateDto dto)
    {
        var admin = await _context.Users.FindAsync(adminId);
        if (admin == null || admin.Role != "Admin")
        {
            throw new UnauthorizedAccessException("Only admins can create events");
        }

        var eventEntity = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            StartAt = dto.StartAt,
            EndAt = dto.EndAt,
            Location = dto.Location,
            CreatedByAdminId = adminId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Events.Add(eventEntity);
        await _context.SaveChangesAsync();

        await _context.Entry(eventEntity).Reference(e => e.CreatedByAdmin).LoadAsync();

        return MapToReadDto(eventEntity);
    }

    public async Task<EventReadDto?> GetEventByIdAsync(int eventId)
    {
        var eventEntity = await _context.Events
            .Include(e => e.CreatedByAdmin)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        return eventEntity == null ? null : MapToReadDto(eventEntity);
    }

    public async Task<List<EventReadDto>> GetAllEventsAsync()
    {
        var events = await _context.Events
            .Include(e => e.CreatedByAdmin)
            .OrderByDescending(e => e.StartAt)
            .ToListAsync();

        return events.Select(MapToReadDto).ToList();
    }

    public async Task InviteVolunteersAsync(int eventId, int adminId, EventInviteDto dto)
    {
        var eventEntity = await _context.Events.FindAsync(eventId);
        if (eventEntity == null)
        {
            throw new ArgumentException("Event not found");
        }

        var admin = await _context.Users.FindAsync(adminId);
        if (admin == null || admin.Role != "Admin")
        {
            throw new UnauthorizedAccessException("Only admins can invite volunteers");
        }

        foreach (var volunteerId in dto.VolunteerIds)
        {
            var volunteer = await _context.Users.FindAsync(volunteerId);
            if (volunteer == null || volunteer.Role != "Volunteer")
            {
                continue;
            }

            var existingParticipation = await _context.EventParticipations
                .FirstOrDefaultAsync(ep => ep.EventId == eventId && ep.VolunteerId == volunteerId);

            if (existingParticipation == null)
            {
                var participation = new EventParticipation
                {
                    EventId = eventId,
                    VolunteerId = volunteerId,
                    Response = "Pending",
                    OccurrenceReport = "Unknown",
                    CreatedAt = DateTime.UtcNow
                };

                _context.EventParticipations.Add(participation);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task RespondToParticipationAsync(int participationId, int volunteerId, ParticipationRespondDto dto)
    {
        var participation = await _context.EventParticipations
            .FirstOrDefaultAsync(ep => ep.Id == participationId);

        if (participation == null)
        {
            throw new ArgumentException("Participation not found");
        }

        if (participation.VolunteerId != volunteerId)
        {
            throw new UnauthorizedAccessException("You can only respond to your own participations");
        }

        if (dto.Response != "Going" && dto.Response != "NotGoing")
        {
            throw new ArgumentException("Response must be 'Going' or 'NotGoing'");
        }

        participation.Response = dto.Response;
        await _context.SaveChangesAsync();
    }

    public async Task ReportOccurrenceAsync(int participationId, int volunteerId, OccurrenceReportDto dto)
    {
        var participation = await _context.EventParticipations
            .FirstOrDefaultAsync(ep => ep.Id == participationId);

        if (participation == null)
        {
            throw new ArgumentException("Participation not found");
        }

        if (participation.VolunteerId != volunteerId)
        {
            throw new UnauthorizedAccessException("You can only report on your own participations");
        }

        if (dto.OccurrenceReport != "Happened" && dto.OccurrenceReport != "DidNotHappen")
        {
            throw new ArgumentException("OccurrenceReport must be 'Happened' or 'DidNotHappen'");
        }

        participation.OccurrenceReport = dto.OccurrenceReport;
        participation.OccurrenceNotes = dto.OccurrenceNotes;
        await _context.SaveChangesAsync();
    }

    public async Task<List<ParticipationReadDto>> GetEventParticipationsAsync(int eventId)
    {
        var participations = await _context.EventParticipations
            .Include(ep => ep.Event)
                .ThenInclude(e => e.CreatedByAdmin)
            .Include(ep => ep.Volunteer)
            .Where(ep => ep.EventId == eventId)
            .ToListAsync();

        return participations.Select(MapParticipationToReadDto).ToList();
    }

    public async Task<List<ParticipationReadDto>> GetVolunteerParticipationsAsync(int volunteerId)
    {
        var participations = await _context.EventParticipations
            .Include(ep => ep.Event)
                .ThenInclude(e => e.CreatedByAdmin)
            .Include(ep => ep.Volunteer)
            .Where(ep => ep.VolunteerId == volunteerId)
            .OrderByDescending(ep => ep.Event.StartAt)
            .ToListAsync();

        return participations.Select(MapParticipationToReadDto).ToList();
    }

    private EventReadDto MapToReadDto(Event eventEntity)
    {
        return new EventReadDto
        {
            Id = eventEntity.Id,
            Title = eventEntity.Title,
            Description = eventEntity.Description,
            StartAt = eventEntity.StartAt,
            EndAt = eventEntity.EndAt,
            Location = eventEntity.Location,
            CreatedByAdminId = eventEntity.CreatedByAdminId,
            CreatedByAdmin = eventEntity.CreatedByAdmin == null ? null : new UserReadDto
            {
                Id = eventEntity.CreatedByAdmin.Id,
                Email = eventEntity.CreatedByAdmin.Email,
                FullName = eventEntity.CreatedByAdmin.FullName,
                Role = eventEntity.CreatedByAdmin.Role,
                CreatedAt = eventEntity.CreatedByAdmin.CreatedAt
            },
            CreatedAt = eventEntity.CreatedAt
        };
    }

    private ParticipationReadDto MapParticipationToReadDto(EventParticipation participation)
    {
        return new ParticipationReadDto
        {
            Id = participation.Id,
            EventId = participation.EventId,
            Event = participation.Event == null ? null : MapToReadDto(participation.Event),
            VolunteerId = participation.VolunteerId,
            Volunteer = participation.Volunteer == null ? null : new UserReadDto
            {
                Id = participation.Volunteer.Id,
                Email = participation.Volunteer.Email,
                FullName = participation.Volunteer.FullName,
                Role = participation.Volunteer.Role,
                CreatedAt = participation.Volunteer.CreatedAt
            },
            Response = participation.Response,
            OccurrenceReport = participation.OccurrenceReport,
            OccurrenceNotes = participation.OccurrenceNotes,
            CreatedAt = participation.CreatedAt
        };
    }
}
