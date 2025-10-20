using Services.Abstract;
using Data.Abstract;
using Entities.DTO;
using Entities.Models;

namespace Services.Concrete;

public class TaskService : GenericService<Entities.Models.Task, TaskPostDto, TaskGetDto, TaskPutDto>, ITaskService
{
    private readonly ITaskRepository _taskRepository;
    
    public TaskService(ITaskRepository taskRepository) : base(taskRepository)
    {
        _taskRepository = taskRepository;
    }
}
