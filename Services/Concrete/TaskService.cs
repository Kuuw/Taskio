using Data.Abstract;
using Entities.Context.Abstract;
using Entities.DTO;
using Entities.Models;
using Services.Abstract;

namespace Services.Concrete;

public class TaskService : GenericService<Entities.Models.Task, TaskPostDto, TaskGetDto, TaskPutDto>, ITaskService
{
    private readonly ITaskRepository _taskRepository;
    protected readonly IUserContext _userContext;

    public TaskService(ITaskRepository taskRepository, IUserContext userContext) : base(taskRepository, userContext)
    {
        _taskRepository = taskRepository;
        _userContext = userContext;
    }
}
