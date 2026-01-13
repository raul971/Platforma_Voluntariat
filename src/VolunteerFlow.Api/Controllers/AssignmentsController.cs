using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VolunteerFlow.Api.DTOs.Assignments;
using VolunteerFlow.Api.Services.Interfaces;

namespace VolunteerFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssignmentsController : ControllerBase
{
    private readonly ITaskService _taskService;

    public AssignmentsController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// Get all assignments (Admin sees all, Volunteer sees only theirs)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAssignments()
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            IEnumerable<AssignmentReadDto> assignments;

            if (userRole == "Admin")
            {
                assignments = await _taskService.GetAllAssignmentsAsync();
            }
            else
            {
                assignments = await _taskService.GetAssignmentsByVolunteerAsync(userId);
            }

            return Ok(assignments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving assignments", error = ex.Message });
        }
    }

    /// <summary>
    /// Get assignment by ID (Volunteer can only access their own)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssignment(int id)
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            var assignment = await _taskService.GetAssignmentByIdAsync(id);

            if (assignment == null)
            {
                return NotFound(new { message = $"Assignment with ID {id} not found" });
            }

            // Volunteers can only see their own assignments
            if (userRole == "Volunteer" && assignment.VolunteerId != userId)
            {
                return Forbid();
            }

            return Ok(assignment);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving assignment", error = ex.Message });
        }
    }

    /// <summary>
    /// Respond to assignment (Accept/Decline) - Volunteer only
    /// </summary>
    [HttpPatch("{id}/respond")]
    [Authorize(Roles = "Volunteer")]
    public async Task<IActionResult> RespondToAssignment(int id, [FromBody] AssignmentRespondDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            // Check if assignment belongs to this volunteer
            var assignment = await _taskService.GetAssignmentByIdAsync(id);

            if (assignment == null)
            {
                return NotFound(new { message = $"Assignment with ID {id} not found" });
            }

            if (assignment.VolunteerId != userId)
            {
                return Forbid();
            }

            var updatedAssignment = await _taskService.RespondToAssignmentAsync(id, dto);

            if (updatedAssignment == null)
            {
                return NotFound(new { message = $"Assignment with ID {id} not found" });
            }

            return Ok(updatedAssignment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while responding to assignment", error = ex.Message });
        }
    }

    /// <summary>
    /// Complete task and report hours - Volunteer only
    /// </summary>
    [HttpPatch("{id}/complete")]
    [Authorize(Roles = "Volunteer")]
    public async Task<IActionResult> CompleteTask(int id, [FromBody] AssignmentCompleteDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            // Check if assignment belongs to this volunteer
            var assignment = await _taskService.GetAssignmentByIdAsync(id);

            if (assignment == null)
            {
                return NotFound(new { message = $"Assignment with ID {id} not found" });
            }

            if (assignment.VolunteerId != userId)
            {
                return Forbid();
            }

            var completedAssignment = await _taskService.CompleteTaskAsync(id, dto);

            if (completedAssignment == null)
            {
                return NotFound(new { message = $"Assignment with ID {id} not found" });
            }

            return Ok(completedAssignment);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while completing task", error = ex.Message });
        }
    }
}
