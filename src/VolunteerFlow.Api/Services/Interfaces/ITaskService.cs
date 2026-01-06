using VolunteerFlow.Api.DTOs.Tasks;
using VolunteerFlow.Api.DTOs.Assignments;

namespace VolunteerFlow.Api.Services.Interfaces;

public interface ITaskService
{
    // CRUD Task-uri (Admin)
    Task<IEnumerable<TaskReadDto>> GetAllTasksAsync();
    Task<TaskReadDto?> GetTaskByIdAsync(int id);
    Task<TaskReadDto> CreateTaskAsync(TaskCreateDto dto, int adminId);
    Task<TaskReadDto?> UpdateTaskAsync(int id, TaskUpdateDto dto);
    Task<bool> DeleteTaskAsync(int id);

    // Asignare task-uri (Admin)
    Task<AssignmentReadDto> AssignTaskAsync(int taskId, int volunteerId);

    // Gestionare asignări
    Task<IEnumerable<AssignmentReadDto>> GetAllAssignmentsAsync();
    Task<IEnumerable<AssignmentReadDto>> GetAssignmentsByVolunteerAsync(int volunteerId);
    Task<AssignmentReadDto?> GetAssignmentByIdAsync(int assignmentId);

    // Răspuns la asignare (Volunteer)
    Task<AssignmentReadDto?> RespondToAssignmentAsync(int assignmentId, AssignmentRespondDto dto);

    // Completare task (Volunteer)
    Task<AssignmentReadDto?> CompleteTaskAsync(int assignmentId, AssignmentCompleteDto dto);
}
