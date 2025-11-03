using Entities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstract;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TaskController : BaseController
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet("FromProjectId/{id}")]
    public IActionResult GetTasksFromProject(Guid id)
    {
        return HandleServiceResult(_taskService.GetTasksFromProject(id));
    }

    [HttpGet("{id}")]
    public IActionResult GetTask(Guid id)
    {
        return HandleServiceResult(_taskService.Get(id));
    }

    [HttpPost]
    public IActionResult PostTask(TaskPostDto task)
    {
        return HandleServiceResult(_taskService.Insert(task));
    }

    [HttpPut]
    public IActionResult PutTask(TaskPutDto task)
    {
        return HandleServiceResult(_taskService.Update(task));
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTask(Guid id)
    {
        return HandleServiceResult(_taskService.Delete(id));
    }

    [HttpPut("AddUser")]
    public IActionResult AddUser(Guid taskId, string email)
    {
        return HandleServiceResult(_taskService.AddUserToTask(taskId, email));
    }

    [HttpPut("RemoveUser")]
    public IActionResult RemoveUser(Guid taskId, string email)
    {
        return HandleServiceResult(_taskService.RemoveUserFromTask(taskId, email));
    }
}
