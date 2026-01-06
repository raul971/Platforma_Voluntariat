using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VolunteerFlow.Api.DTOs.Meetings;
using VolunteerFlow.Api.Services.Interfaces;

namespace VolunteerFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MeetingsController : ControllerBase
{
    private readonly IMeetingService _meetingService;

    public MeetingsController(IMeetingService meetingService)
    {
        _meetingService = meetingService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateMeeting([FromBody] MeetingCreateDto dto)
    {
        try
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var meeting = await _meetingService.CreateMeetingAsync(adminId, dto);
            return CreatedAtAction(nameof(GetMeetingById), new { id = meeting.Id }, meeting);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating meeting", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMeetingById(int id)
    {
        try
        {
            var meeting = await _meetingService.GetMeetingByIdAsync(id);
            if (meeting == null)
            {
                return NotFound(new { message = "Meeting not found" });
            }
            return Ok(meeting);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving meeting", error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMeetings()
    {
        try
        {
            var meetings = await _meetingService.GetAllMeetingsAsync();
            return Ok(meetings);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving meetings", error = ex.Message });
        }
    }

    [HttpPost("{id}/invite")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> InviteVolunteers(int id, [FromBody] MeetingInviteDto dto)
    {
        try
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _meetingService.InviteVolunteersAsync(id, adminId, dto);
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

    [HttpGet("{id}/invitations")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetMeetingInvitations(int id)
    {
        try
        {
            var invitations = await _meetingService.GetMeetingInvitationsAsync(id);
            return Ok(invitations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving invitations", error = ex.Message });
        }
    }
}
