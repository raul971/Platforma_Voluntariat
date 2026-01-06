using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VolunteerFlow.Api.Services.Interfaces;

namespace VolunteerFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Get volunteer hours report by ID (Admin can view any, Volunteer can view their own)
    /// </summary>
    [HttpGet("volunteers/{id}")]
    public async Task<IActionResult> GetVolunteerReport(int id)
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            // Volunteers can only view their own report
            if (userRole == "Volunteer" && userId != id)
            {
                return Forbid();
            }

            var report = await _reportService.GetVolunteerHoursReportAsync(id);

            if (report == null)
            {
                return NotFound(new { message = $"Volunteer with ID {id} not found" });
            }

            return Ok(report);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving report", error = ex.Message });
        }
    }

    /// <summary>
    /// Get all volunteers' hours reports (Admin only)
    /// </summary>
    [HttpGet("volunteers")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllVolunteersReports()
    {
        try
        {
            var reports = await _reportService.GetAllVolunteersHoursReportAsync();
            return Ok(reports);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving reports", error = ex.Message });
        }
    }
}
