using Entities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstract;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ProjectController : BaseController
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleServiceResult(await _projectService.GetAsync());
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] ProjectPostDto data)
    {
        return HandleServiceResult(await _projectService.CreateAsync(data));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProject([FromBody] ProjectPutDto data)
    {
        return HandleServiceResult(await _projectService.UpdateAsync(data));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        return HandleServiceResult(await _projectService.DeleteAsync(id));
    }

    [HttpPut]
    [Route("AddUser")]
    public async Task<IActionResult> AddUserToProject([FromQuery] Guid projectId, [FromQuery] string email)
    {
        return HandleServiceResult(await _projectService.AddUserToProjectAsync(projectId, email));
    }

    [HttpPut]
    [Route("RemoveUser")]
    public async Task<IActionResult> RemoveUserFromProject([FromQuery] Guid projectId, [FromQuery] string email)
    {
        return HandleServiceResult(await _projectService.RemoveUserFromProjectAsync(projectId, email));
    }

    [HttpPut]
    [Route("MakeAdmin")]
    public async Task<IActionResult> ChangeUserProjectAdmin([FromQuery] Guid projectId, [FromQuery] Guid userId)
    {
        return HandleServiceResult(await _projectService.SetUserAsAdminAsync(projectId, userId, true));
    }
}
