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
    public async Task<IActionResult> GetTasksFromProject(Guid id)
    {
        return HandleServiceResult(await _taskService.GetTasksFromProjectAsync(id));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(Guid id)
    {
        return HandleServiceResult(await _taskService.GetAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> PostTask(TaskPostDto task)
    {
        return HandleServiceResult(await _taskService.InsertAsync(task));
    }

    [HttpPut]
    public async Task<IActionResult> PutTask(TaskPutDto task)
    {
        return HandleServiceResult(await _taskService.UpdateAsync(task));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        return HandleServiceResult(await _taskService.DeleteAsync(id));
    }

    [HttpPut("AddUser")]
    public async Task<IActionResult> AddUser(Guid taskId, string email)
    {
        return HandleServiceResult(await _taskService.AddUserToTaskAsync(taskId, email));
    }

    [HttpPut("RemoveUser")]
    public async Task<IActionResult> RemoveUser(Guid taskId, string email)
    {
        return HandleServiceResult(await _taskService.RemoveUserFromTaskAsync(taskId, email));
    }
}
