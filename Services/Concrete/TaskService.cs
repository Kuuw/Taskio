using AutoMapper;
using Data.Abstract;
using Data.Concrete;
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
    private readonly Mapper _mapper = MapperConfig.InitializeAutomapper();

    public TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository, IUserRepository userRepository, IUserContext userContext) : base(taskRepository, userContext)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public ServiceResult<TaskGetDto> Get(Guid guid)
    {
        var access = ValidateProjectAccess(guid, "get");
        if (access != null)
        {
            return ServiceResult<TaskGetDto>.BadRequest(access.ErrorMessage ?? "Bad Request");
        }
        var task = _taskRepository.GetById(guid);
        return ServiceResult<TaskGetDto>.Ok(_mapper.Map<TaskGetDto>(task));
    }

    public ServiceResult<List<TaskGetDto>> GetTasksFromProject(Guid guid)
    {
        var access = ValidateProjectAccess(guid, "get");
        if (access != null)
        {
            return ServiceResult<List<TaskGetDto>>.BadRequest(access.ErrorMessage ?? "Bad Request");
        }
        var tasks = _projectRepository.GetById(guid)!.Tasks;
        return ServiceResult<List<TaskGetDto>>.Ok(_mapper.Map<List<TaskGetDto>>(tasks));
    }

    public new ServiceResult<TaskGetDto> Insert(TaskPostDto taskPostDto)
    {
        var access = ValidateProjectAccess(taskPostDto.ProjectId, "create in");
        if (access != null)
        {
            return ServiceResult<TaskGetDto>.BadRequest(access.ErrorMessage ?? "Bad Request");
        }
        var task = _mapper.Map<Entities.Models.Task>(taskPostDto);
        var result = _taskRepository.Insert(task);
        
        if (result)
        {
            // Fetch the created task to return full DTO
            var tasks = _taskRepository.Where(new List<Func<Entities.Models.Task, bool>> { x => x.CategoryId == taskPostDto.CategoryId });
            var createdTask = tasks.OrderByDescending(t => t.CreatedAt).FirstOrDefault();
            
            if (createdTask != null)
            {
                var taskDto = _mapper.Map<TaskGetDto>(createdTask);
                return ServiceResult<TaskGetDto>.Ok(taskDto);
            }
        }
        
        return ServiceResult<TaskGetDto>.InternalServerError("Failed to create task.");
    }

    public new ServiceResult<TaskGetDto> Update(TaskPutDto taskPutDto)
    {
        var existingTask = _taskRepository.GetById(taskPutDto.TaskId);
        if (existingTask == null)
        {
            return ServiceResult<TaskGetDto>.NotFound("Task not found.");
        }
        var access = ValidateProjectAccess(existingTask.ProjectId, "update in");
        if (access != null)
        {
            return ServiceResult<TaskGetDto>.BadRequest(access.ErrorMessage ?? "Bad Request");
        }
        var task = _mapper.Map<Entities.Models.Task>(taskPutDto);
        task.UpdatedAt = DateTime.UtcNow;
        var result = _taskRepository.Update(task);
        
        if (result)
        {
            var updatedTask = _taskRepository.GetById(taskPutDto.TaskId);
            if (updatedTask != null)
            {
                var taskDto = _mapper.Map<TaskGetDto>(updatedTask);
                return ServiceResult<TaskGetDto>.Ok(taskDto);
            }
        }
        
        return ServiceResult<TaskGetDto>.InternalServerError("Failed to update task.");
    }

    public new ServiceResult<bool> Delete(Guid taskId)
    {
        var existingTask = _taskRepository.GetById(taskId);
        if (existingTask == null)
        {
            return ServiceResult<bool>.NotFound("Task not found.");
        }
        var access = ValidateProjectAccess(existingTask.ProjectId, "delete from");
        if (access != null)
        {
            return ServiceResult<bool>.BadRequest(access.ErrorMessage ?? "Bad Request");
        }
        var result = _taskRepository.Delete(existingTask);
        return ServiceResult<bool>.Ok(result);
    }

    public ServiceResult<bool> AddUserToTask(Guid taskId, string email)
    {
        var task = _taskRepository.GetById(taskId);
        if (task == null)
        {
            return ServiceResult<bool>.NotFound("Task not found.");
        }

        var access = ValidateProjectAccess(task.ProjectId, "assign users in");
        if (access != null)
        {
            return ServiceResult<bool>.BadRequest(access.ErrorMessage ?? "Bad Request");
        }

        var user = _userRepository.GetByEmail(email);
        if (user == null)
        {
            return ServiceResult<bool>.NotFound("User not found.");
        }

        var project = _projectRepository.GetById(task.ProjectId);
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
        var result = _taskRepository.Update(task);

        return ServiceResult<bool>.Ok(result);
    }

    public ServiceResult<bool> RemoveUserFromTask(Guid taskId, string email)
    {
        var task = _taskRepository.GetById(taskId);
        if (task == null)
        {
            return ServiceResult<bool>.NotFound("Task not found.");
        }

        var access = ValidateProjectAccess(task.ProjectId, "remove users from");
        if (access != null)
        {
            return ServiceResult<bool>.BadRequest(access.ErrorMessage ?? "Bad Request");
        }

        var user = task.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            return ServiceResult<bool>.NotFound("User is not assigned to this task.");
        }

        task.Users.Remove(user);
        task.UpdatedAt = DateTime.UtcNow;
        var result = _taskRepository.Update(task);

        return ServiceResult<bool>.Ok(result);
    }

    private ServiceResult<bool>? ValidateProjectAccess(Guid projectId, string operation)
    {
        var project = _projectRepository.GetById(projectId);
        if (project == null)
        {
            return ServiceResult<bool>.NotFound("Project not found.");
        }
        if (!project.ProjectUsers.Any(pu => pu.UserId == _userContext.UserId))
        {
            return ServiceResult<bool>.Unauthorized($"You do not have permission to {operation} this project.");
        }
        return null;
    }
}
