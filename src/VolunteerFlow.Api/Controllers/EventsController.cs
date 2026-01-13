using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VolunteerFlow.Api.DTOs.Events;
using VolunteerFlow.Api.Services.Interfaces;

namespace VolunteerFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateEvent([FromBody] EventCreateDto dto)
    {
        try
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var eventDto = await _eventService.CreateEventAsync(adminId, dto);
            return CreatedAtAction(nameof(GetEventById), new { id = eventDto.Id }, eventDto);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating event", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEventById(int id)
    {
        try
        {
            var eventDto = await _eventService.GetEventByIdAsync(id);
            if (eventDto == null)
            {
                return NotFound(new { message = "Event not found" });
            }
            return Ok(eventDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving event", error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEvents()
    {
        try
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving events", error = ex.Message });
        }
    }

    [HttpPost("{id}/invite")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> InviteVolunteers(int id, [FromBody] EventInviteDto dto)
    {
        try
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _eventService.InviteVolunteersAsync(id, adminId, dto);
            return Ok(new { message = "Volunteers invited successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error inviting volunteers", error = ex.Message });
        }
    }

    [HttpGet("{id}/participations")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetEventParticipations(int id)
    {
        try
        {
            var participations = await _eventService.GetEventParticipationsAsync(id);
            return Ok(participations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving participations", error = ex.Message });
        }
    }
}
