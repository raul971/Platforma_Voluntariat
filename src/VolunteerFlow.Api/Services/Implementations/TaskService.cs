using Microsoft.EntityFrameworkCore;
using VolunteerFlow.Api.Data;
using VolunteerFlow.Api.DTOs.Tasks;
using VolunteerFlow.Api.DTOs.Assignments;
using VolunteerFlow.Api.DTOs.Users;
using VolunteerFlow.Api.Models;
using VolunteerFlow.Api.Services.Interfaces;

namespace VolunteerFlow.Api.Services.Implementations;

public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _context;

    public TaskService(ApplicationDbContext context)
    {
        _context = context;
    }

    // ============ CRUD Task-uri (Admin) ============

    public async Task<IEnumerable<TaskReadDto>> GetAllTasksAsync()
    {
        var tasks = await _context.Tasks
            .Include(t => t.CreatedByAdmin)
            .Select(t => new TaskReadDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                EstimatedHours = t.EstimatedHours,
                Deadline = t.Deadline,
                CreatedByAdminId = t.CreatedByAdminId,
                CreatedByAdmin = new UserReadDto
                {
                    Id = t.CreatedByAdmin.Id,
                    Email = t.CreatedByAdmin.Email,
                    FullName = t.CreatedByAdmin.FullName,
                    Role = t.CreatedByAdmin.Role,
                    CreatedAt = t.CreatedByAdmin.CreatedAt
                },
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();

        return tasks;
    }

    public async Task<TaskReadDto?> GetTaskByIdAsync(int id)
    {
        var task = await _context.Tasks
            .Include(t => t.CreatedByAdmin)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return null;
        }

        return new TaskReadDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            EstimatedHours = task.EstimatedHours,
            Deadline = task.Deadline,
            CreatedByAdminId = task.CreatedByAdminId,
            CreatedByAdmin = new UserReadDto
            {
                Id = task.CreatedByAdmin.Id,
                Email = task.CreatedByAdmin.Email,
                FullName = task.CreatedByAdmin.FullName,
                Role = task.CreatedByAdmin.Role,
                CreatedAt = task.CreatedByAdmin.CreatedAt
            },
            CreatedAt = task.CreatedAt
        };
    }

    public async Task<TaskReadDto> CreateTaskAsync(TaskCreateDto dto, int adminId)
    {
        // Validate input
        if (string.IsNullOrEmpty(dto.Title) || string.IsNullOrEmpty(dto.Description))
        {
            throw new ArgumentException("Title and description are required");
        }

        if (dto.EstimatedHours <= 0)
        {
            throw new ArgumentException("Estimated hours must be greater than 0");
        }

        if (dto.Deadline <= DateTime.UtcNow)
        {
            throw new ArgumentException("Deadline must be in the future");
        }

        // Create task
        var task = new TaskModel
        {
            Title = dto.Title,
            Description = dto.Description,
            EstimatedHours = dto.EstimatedHours,
            Deadline = dto.Deadline,
            CreatedByAdminId = adminId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Load admin for response
        await _context.Entry(task).Reference(t => t.CreatedByAdmin).LoadAsync();

        return new TaskReadDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            EstimatedHours = task.EstimatedHours,
            Deadline = task.Deadline,
            CreatedByAdminId = task.CreatedByAdminId,
            CreatedByAdmin = new UserReadDto
            {
                Id = task.CreatedByAdmin.Id,
                Email = task.CreatedByAdmin.Email,
                FullName = task.CreatedByAdmin.FullName,
                Role = task.CreatedByAdmin.Role,
                CreatedAt = task.CreatedByAdmin.CreatedAt
            },
            CreatedAt = task.CreatedAt
        };
    }

    public async Task<TaskReadDto?> UpdateTaskAsync(int id, TaskUpdateDto dto)
    {
        var task = await _context.Tasks
            .Include(t => t.CreatedByAdmin)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return null;
        }

        // Update only provided fields
        if (!string.IsNullOrEmpty(dto.Title))
        {
            task.Title = dto.Title;
        }

        if (!string.IsNullOrEmpty(dto.Description))
        {
            task.Description = dto.Description;
        }

        if (dto.EstimatedHours.HasValue && dto.EstimatedHours.Value > 0)
        {
            task.EstimatedHours = dto.EstimatedHours.Value;
        }

        if (dto.Deadline.HasValue)
        {
            if (dto.Deadline.Value <= DateTime.UtcNow)
            {
                throw new ArgumentException("Deadline must be in the future");
            }
            task.Deadline = dto.Deadline.Value;
        }

        await _context.SaveChangesAsync();

        return new TaskReadDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            EstimatedHours = task.EstimatedHours,
            Deadline = task.Deadline,
            CreatedByAdminId = task.CreatedByAdminId,
            CreatedByAdmin = new UserReadDto
            {
                Id = task.CreatedByAdmin.Id,
                Email = task.CreatedByAdmin.Email,
                FullName = task.CreatedByAdmin.FullName,
                Role = task.CreatedByAdmin.Role,
                CreatedAt = task.CreatedByAdmin.CreatedAt
            },
            CreatedAt = task.CreatedAt
        };
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
        {
            return false;
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return true;
    }

    // ============ Asignare Task-uri (Admin) ============

    public async Task<AssignmentReadDto> AssignTaskAsync(int taskId, int volunteerId)
    {
        // Validate task exists
        var task = await _context.Tasks
            .Include(t => t.CreatedByAdmin)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
        {
            throw new InvalidOperationException($"Task with ID {taskId} not found");
        }

        // Validate volunteer exists
        var volunteer = await _context.Users.FindAsync(volunteerId);

        if (volunteer == null)
        {
            throw new InvalidOperationException($"Volunteer with ID {volunteerId} not found");
        }

        if (volunteer.Role != "Volunteer")
        {
            throw new InvalidOperationException("User must be a volunteer");
        }

        // Check if already assigned
        var existingAssignment = await _context.TaskAssignments
            .FirstOrDefaultAsync(a => a.TaskId == taskId && a.VolunteerId == volunteerId);

        if (existingAssignment != null)
        {
            throw new InvalidOperationException("Task is already assigned to this volunteer");
        }

        // Create assignment
        var assignment = new TaskAssignment
        {
            TaskId = taskId,
            VolunteerId = volunteerId,
            Status = "Assigned",
            CreatedAt = DateTime.UtcNow
        };

        _context.TaskAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        // Load related data
        await _context.Entry(assignment).Reference(a => a.Task).LoadAsync();
        await _context.Entry(assignment).Reference(a => a.Volunteer).LoadAsync();
        await _context.Entry(assignment.Task).Reference(t => t.CreatedByAdmin).LoadAsync();

        return MapToAssignmentReadDto(assignment);
    }

    // ============ Gestionare Asignări ============

    public async Task<IEnumerable<AssignmentReadDto>> GetAllAssignmentsAsync()
    {
        var assignments = await _context.TaskAssignments
            .Include(a => a.Task)
                .ThenInclude(t => t.CreatedByAdmin)
            .Include(a => a.Volunteer)
            .ToListAsync();

        return assignments.Select(MapToAssignmentReadDto);
    }

    public async Task<IEnumerable<AssignmentReadDto>> GetAssignmentsByVolunteerAsync(int volunteerId)
    {
        var assignments = await _context.TaskAssignments
            .Include(a => a.Task)
                .ThenInclude(t => t.CreatedByAdmin)
            .Include(a => a.Volunteer)
            .Where(a => a.VolunteerId == volunteerId)
            .ToListAsync();

        return assignments.Select(MapToAssignmentReadDto);
    }

    public async Task<AssignmentReadDto?> GetAssignmentByIdAsync(int assignmentId)
    {
        var assignment = await _context.TaskAssignments
            .Include(a => a.Task)
                .ThenInclude(t => t.CreatedByAdmin)
            .Include(a => a.Volunteer)
            .FirstOrDefaultAsync(a => a.Id == assignmentId);

        if (assignment == null)
        {
            return null;
        }

        return MapToAssignmentReadDto(assignment);
    }

    // ============ Răspuns la Asignare (Volunteer) ============

    public async Task<AssignmentReadDto?> RespondToAssignmentAsync(int assignmentId, AssignmentRespondDto dto)
    {
        var assignment = await _context.TaskAssignments
            .Include(a => a.Task)
                .ThenInclude(t => t.CreatedByAdmin)
            .Include(a => a.Volunteer)
            .FirstOrDefaultAsync(a => a.Id == assignmentId);

        if (assignment == null)
        {
            return null;
        }

        if (assignment.Status != "Assigned")
        {
            throw new InvalidOperationException("Can only respond to assigned tasks");
        }

        // Update assignment based on response
        if (dto.Accepted)
        {
            assignment.Status = "Accepted";
            assignment.DeclineReason = null;
        }
        else
        {
            assignment.Status = "Declined";
            assignment.DeclineReason = dto.DeclineReason;
        }

        await _context.SaveChangesAsync();

        return MapToAssignmentReadDto(assignment);
    }

    // ============ Completare Task (Volunteer) ============

    public async Task<AssignmentReadDto?> CompleteTaskAsync(int assignmentId, AssignmentCompleteDto dto)
    {
        var assignment = await _context.TaskAssignments
            .Include(a => a.Task)
                .ThenInclude(t => t.CreatedByAdmin)
            .Include(a => a.Volunteer)
            .FirstOrDefaultAsync(a => a.Id == assignmentId);

        if (assignment == null)
        {
            return null;
        }

        if (assignment.Status != "Accepted")
        {
            throw new InvalidOperationException("Can only complete accepted tasks");
        }

        if (dto.WorkedHours <= 0)
        {
            throw new ArgumentException("Worked hours must be greater than 0");
        }

        // Mark as completed
        assignment.Status = "Completed";
        assignment.WorkedHours = dto.WorkedHours;
        assignment.Notes = dto.Notes;
        assignment.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToAssignmentReadDto(assignment);
    }

    // ============ Helper Methods ============

    private AssignmentReadDto MapToAssignmentReadDto(TaskAssignment assignment)
    {
        return new AssignmentReadDto
        {
            Id = assignment.Id,
            TaskId = assignment.TaskId,
            Task = new TaskReadDto
            {
                Id = assignment.Task.Id,
                Title = assignment.Task.Title,
                Description = assignment.Task.Description,
                EstimatedHours = assignment.Task.EstimatedHours,
                Deadline = assignment.Task.Deadline,
                CreatedByAdminId = assignment.Task.CreatedByAdminId,
                CreatedByAdmin = new UserReadDto
                {
                    Id = assignment.Task.CreatedByAdmin.Id,
                    Email = assignment.Task.CreatedByAdmin.Email,
                    FullName = assignment.Task.CreatedByAdmin.FullName,
                    Role = assignment.Task.CreatedByAdmin.Role,
                    CreatedAt = assignment.Task.CreatedByAdmin.CreatedAt
                },
                CreatedAt = assignment.Task.CreatedAt
            },
            VolunteerId = assignment.VolunteerId,
            Volunteer = new UserReadDto
            {
                Id = assignment.Volunteer.Id,
                Email = assignment.Volunteer.Email,
                FullName = assignment.Volunteer.FullName,
                Role = assignment.Volunteer.Role,
                CreatedAt = assignment.Volunteer.CreatedAt
            },
            Status = assignment.Status,
            DeclineReason = assignment.DeclineReason,
            CompletedAt = assignment.CompletedAt,
            WorkedHours = assignment.WorkedHours,
            Notes = assignment.Notes,
            CreatedAt = assignment.CreatedAt
        };
    }
}
