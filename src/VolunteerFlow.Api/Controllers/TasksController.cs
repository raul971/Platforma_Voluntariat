using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VolunteerFlow.Api.DTOs.Tasks;
using VolunteerFlow.Api.Services.Interfaces;

namespace VolunteerFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// Get all tasks (All users can view)
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllTasks()
    {
        try
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving tasks", error = ex.Message });
        }
    }

    /// <summary>
    /// Get task by ID (All users can view)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetTask(int id)
    {
        try
        {
            var task = await _taskService.GetTaskByIdAsync(id);

            if (task == null)
            {
                return NotFound(new { message = $"Task with ID {id} not found" });
            }

            return Ok(task);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving task", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new task (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateTask([FromBody] TaskCreateDto dto)
    {
        try
        {
            // Get admin ID from claims
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out int adminId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            var task = await _taskService.CreateTaskAsync(dto, adminId);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating task", error = ex.Message });
        }
    }

    /// <summary>
    /// Update task (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskUpdateDto dto)
    {
        try
        {
            var task = await _taskService.UpdateTaskAsync(id, dto);

            if (task == null)
            {
                return NotFound(new { message = $"Task with ID {id} not found" });
            }

            return Ok(task);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating task", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete task (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            var result = await _taskService.DeleteTaskAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Task with ID {id} not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting task", error = ex.Message });
        }
    }

    /// <summary>
    /// Assign task to volunteer (Admin only)
    /// </summary>
    [HttpPost("{taskId}/assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignTask(int taskId, [FromBody] TaskAssignDto dto)
    {
        try
        {
            var assignment = await _taskService.AssignTaskAsync(taskId, dto.VolunteerId);
            return CreatedAtAction(nameof(GetTask), new { id = taskId }, assignment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while assigning task", error = ex.Message });
        }
    }
}
