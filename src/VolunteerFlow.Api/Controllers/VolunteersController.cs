using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerFlow.Api.DTOs.Users;
using VolunteerFlow.Api.Services.Interfaces;

namespace VolunteerFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class VolunteersController : ControllerBase
{
    private readonly IUserService _userService;

    public VolunteersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get all volunteers (Admin only)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllVolunteers()
    {
        try
        {
            var volunteers = await _userService.GetAllVolunteersAsync();
            return Ok(volunteers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving volunteers", error = ex.Message });
        }
    }

    /// <summary>
    /// Get volunteer by ID (Admin only)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetVolunteer(int id)
    {
        try
        {
            var volunteer = await _userService.GetByIdAsync(id);

            if (volunteer == null)
            {
                return NotFound(new { message = $"Volunteer with ID {id} not found" });
            }

            return Ok(volunteer);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving volunteer", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new volunteer (Admin only)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateVolunteer([FromBody] UserCreateDto dto)
    {
        try
        {
            var volunteer = await _userService.CreateVolunteerAsync(dto);
            return CreatedAtAction(nameof(GetVolunteer), new { id = volunteer.Id }, volunteer);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating volunteer", error = ex.Message });
        }
    }

    /// <summary>
    /// Update volunteer (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVolunteer(int id, [FromBody] UserUpdateDto dto)
    {
        try
        {
            var volunteer = await _userService.UpdateAsync(id, dto);

            if (volunteer == null)
            {
                return NotFound(new { message = $"Volunteer with ID {id} not found" });
            }

            return Ok(volunteer);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating volunteer", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete volunteer (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVolunteer(int id)
    {
        try
        {
            var result = await _userService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Volunteer with ID {id} not found" });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting volunteer", error = ex.Message });
        }
    }
}
