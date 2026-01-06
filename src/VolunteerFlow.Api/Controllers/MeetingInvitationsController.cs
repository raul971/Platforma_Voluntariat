using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VolunteerFlow.Api.DTOs.MeetingInvitations;
using VolunteerFlow.Api.Services.Interfaces;

namespace VolunteerFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MeetingInvitationsController : ControllerBase
{
    private readonly IMeetingService _meetingService;

    public MeetingInvitationsController(IMeetingService meetingService)
    {
        _meetingService = meetingService;
    }

    [HttpGet("my-invitations")]
    [Authorize(Roles = "Volunteer")]
    public async Task<IActionResult> GetMyInvitations()
    {
        try
        {
            var volunteerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var invitations = await _meetingService.GetVolunteerInvitationsAsync(volunteerId);
            return Ok(invitations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving invitations", error = ex.Message });
        }
    }

    [HttpPut("{id}/respond")]
    [Authorize(Roles = "Volunteer")]
    public async Task<IActionResult> RespondToInvitation(int id, [FromBody] InvitationRespondDto dto)
    {
        try
        {
            var volunteerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _meetingService.RespondToInvitationAsync(id, volunteerId, dto);
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
            return StatusCode(500, new { message = "Error responding to invitation", error = ex.Message });
        }
    }

    [HttpPut("{id}/attendance")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MarkAttendance(int id, [FromBody] AttendanceDto dto)
    {
        try
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _meetingService.MarkAttendanceAsync(id, adminId, dto);
            return Ok(new { message = "Attendance marked successfully" });
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
            return StatusCode(500, new { message = "Error marking attendance", error = ex.Message });
        }
    }
}
