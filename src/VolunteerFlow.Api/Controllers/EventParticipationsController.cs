using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VolunteerFlow.Api.DTOs.EventParticipations;
using VolunteerFlow.Api.Services.Interfaces;

namespace VolunteerFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventParticipationsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventParticipationsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet("my-participations")]
    [Authorize(Roles = "Volunteer")]
    public async Task<IActionResult> GetMyParticipations()
    {
        try
        {
            var volunteerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var participations = await _eventService.GetVolunteerParticipationsAsync(volunteerId);
            return Ok(participations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving participations", error = ex.Message });
        }
    }

    [HttpPut("{id}/respond")]
    [Authorize(Roles = "Volunteer")]
    public async Task<IActionResult> RespondToParticipation(int id, [FromBody] ParticipationRespondDto dto)
    {
        try
        {
            var volunteerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _eventService.RespondToParticipationAsync(id, volunteerId, dto);
            return Ok(new { message = "Response recorded successfully" });
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
            return StatusCode(500, new { message = "Error responding to participation", error = ex.Message });
        }
    }

    [HttpPut("{id}/report")]
    [Authorize(Roles = "Volunteer")]
    public async Task<IActionResult> ReportOccurrence(int id, [FromBody] OccurrenceReportDto dto)
    {
        try
        {
            var volunteerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _eventService.ReportOccurrenceAsync(id, volunteerId, dto);
            return Ok(new { message = "Occurrence report submitted successfully" });
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
            return StatusCode(500, new { message = "Error reporting occurrence", error = ex.Message });
        }
    }
}
