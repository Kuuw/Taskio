using AutoMapper;
using Data.Abstract;
using Entities.Context.Abstract;
using Entities.DTO;
using Entities.Models;
using Services.Abstract;

namespace Services.Concrete;

public class TaskService : GenericService<Entities.Models.Task, TaskPostDto, TaskGetDto, TaskPutDto>, ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    protected readonly IUserContext _userContext;

    public TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository, IUserRepository userRepository, IUserContext userContext, IMapper mapper) 
        : base(taskRepository, userContext, mapper)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public async Task<ServiceResult<TaskGetDto>> GetAsync(Guid guid)
    {
        var task = await _taskRepository.GetByIdAsync(guid);
        if (task == null)
        {
            return ServiceResult<TaskGetDto>.NotFound("Task not found.");
        }
        
        var access = await ValidateProjectAccessAsync(task.ProjectId, "get");
        if (!access.Success)
        {
            return new ServiceResult<TaskGetDto> { ErrorMessage = access.ErrorMessage, StatusCode = access.StatusCode, Success = false };
        }
        
        return ServiceResult<TaskGetDto>.Ok(_mapper.Map<TaskGetDto>(task));
    }

    public async Task<ServiceResult<List<TaskGetDto>>> GetTasksFromProjectAsync(Guid guid)
    {
        var access = await ValidateProjectAccessAsync(guid, "get");
        if (!access.Success)
        {
            return new ServiceResult<List<TaskGetDto>> { ErrorMessage = access.ErrorMessage, StatusCode = access.StatusCode, Success = false };
        }
        var project = await _projectRepository.GetByIdAsync(guid);
        var tasks = project!.Tasks;
        return ServiceResult<List<TaskGetDto>>.Ok(_mapper.Map<List<TaskGetDto>>(tasks));
    }

    public new async Task<ServiceResult<TaskGetDto>> InsertAsync(TaskPostDto taskPostDto)
    {
        var access = await ValidateProjectAccessAsync(taskPostDto.ProjectId, "create in");
        if (!access.Success)
        {
            return new ServiceResult<TaskGetDto> { ErrorMessage = access.ErrorMessage, StatusCode = access.StatusCode, Success = false };
        }
        var task = _mapper.Map<Entities.Models.Task>(taskPostDto);
        var result = await _taskRepository.InsertAsync(task);
        
        if (result)
        {
            // Fetch the created task to return full DTO
            var tasks = await _taskRepository.WhereAsync(new List<Func<Entities.Models.Task, bool>> { x => x.CategoryId == taskPostDto.CategoryId });
            var createdTask = tasks.OrderByDescending(t => t.CreatedAt).FirstOrDefault();
            
            if (createdTask != null)
            {
                var taskDto = _mapper.Map<TaskGetDto>(createdTask);
                return ServiceResult<TaskGetDto>.Ok(taskDto);
            }
        }
        
        return ServiceResult<TaskGetDto>.InternalServerError("Failed to create task.");
    }

    public new async Task<ServiceResult<TaskGetDto>> UpdateAsync(TaskPutDto taskPutDto)
    {
        var existingTask = await _taskRepository.GetByIdAsync(taskPutDto.TaskId);
        if (existingTask == null)
        {
            return ServiceResult<TaskGetDto>.NotFound("Task not found.");
        }
        var access = await ValidateProjectAccessAsync(existingTask.ProjectId, "update in");
        if (!access.Success)
        {
            return new ServiceResult<TaskGetDto> { ErrorMessage = access.ErrorMessage, StatusCode = access.StatusCode, Success = false };
        }
        var task = _mapper.Map<Entities.Models.Task>(taskPutDto);
        task.UpdatedAt = DateTime.UtcNow;
        var result = await _taskRepository.UpdateAsync(task);
        
        if (result)
        {
            var updatedTask = await _taskRepository.GetByIdAsync(taskPutDto.TaskId);
            if (updatedTask != null)
            {
                var taskDto = _mapper.Map<TaskGetDto>(updatedTask);
                return ServiceResult<TaskGetDto>.Ok(taskDto);
            }
        }
        
        return ServiceResult<TaskGetDto>.InternalServerError("Failed to update task.");
    }

    public override async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        var existingTask = await _taskRepository.GetByIdAsync(id);
        if (existingTask == null)
        {
            return ServiceResult<bool>.NotFound("Task not found.");
        }
        var access = await ValidateProjectAccessAsync(existingTask.ProjectId, "delete from");
        if (!access.Success)
        {
            return access;
        }
        var result = await _taskRepository.DeleteAsync(existingTask);
        return ServiceResult<bool>.Ok(result);
    }

    public async Task<ServiceResult<bool>> AddUserToTaskAsync(Guid taskId, string email)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
        {
            return ServiceResult<bool>.NotFound("Task not found.");
        }

        var access = await ValidateProjectAccessAsync(task.ProjectId, "assign users in");
        if (!access.Success)
        {
            return access;
        }

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return ServiceResult<bool>.NotFound("User not found.");
        }

        var project = await _projectRepository.GetByIdAsync(task.ProjectId);
        if (!project!.ProjectUsers.Any(pu => pu.UserId == user.UserId))
        {
            return ServiceResult<bool>.BadRequest("User is not a member of the project.");
        }

        if (task.Users.Any(u => u.UserId == user.UserId))
        {
            return ServiceResult<bool>.Conflict("User is already assigned to this task.");
        }

        task.Users.Add(user);
        task.UpdatedAt = DateTime.UtcNow;
        var result = await _taskRepository.UpdateAsync(task);

        return ServiceResult<bool>.Ok(result);
    }

    public async Task<ServiceResult<bool>> RemoveUserFromTaskAsync(Guid taskId, string email)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
        {
            return ServiceResult<bool>.NotFound("Task not found.");
        }

        var access = await ValidateProjectAccessAsync(task.ProjectId, "remove users from");
        if (!access.Success)
        {
            return access;
        }

        var user = task.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            return ServiceResult<bool>.NotFound("User is not assigned to this task.");
        }

        task.Users.Remove(user);
        task.UpdatedAt = DateTime.UtcNow;
        var result = await _taskRepository.UpdateAsync(task);

        return ServiceResult<bool>.Ok(result);
    }

    private async Task<ServiceResult<bool>> ValidateProjectAccessAsync(Guid projectId, string operation)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
        {
            return ServiceResult<bool>.NotFound("Project not found.");
        }
        if (!project.ProjectUsers.Any(pu => pu.UserId == _userContext.UserId))
        {
            return ServiceResult<bool>.Unauthorized($"You do not have permission to {operation} this project.");
        }
        return ServiceResult<bool>.Ok(true);
    }
}