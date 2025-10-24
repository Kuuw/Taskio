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
    public IActionResult GetAll()
    {
        return HandleServiceResult(_projectService.Get());
    }

    [HttpPost]
    public IActionResult CreateProject([FromBody] ProjectPostDto data)
    {
        return HandleServiceResult(_projectService.Create(data));
    }

    [HttpPut]
    public IActionResult UpdateProject([FromBody] ProjectPutDto data)
    {
        return HandleServiceResult(_projectService.Update(data));
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteProject(Guid id)
    {
        return HandleServiceResult(_projectService.Delete(id));
    }

    [HttpPut]
    [Route("AddUser")]
    public IActionResult AddUserToProject([FromQuery] Guid projectId, [FromQuery] Guid userId)
    {
        return HandleServiceResult(_projectService.AddUserToProject(projectId, userId));
    }

    [HttpPut]
    [Route("RemoveUser")]
    public IActionResult RemoveUserFromProject([FromQuery] Guid projectId, [FromQuery] Guid userId)
    {
        return HandleServiceResult(_projectService.RemoveUserFromProject(projectId, userId));
    }

    [HttpPut]
    [Route("MakeAdmin")]
    public IActionResult ChangeUserProjectAdmin([FromQuery] Guid projectId, [FromQuery] Guid userId)
    {
        return HandleServiceResult(_projectService.SetUserAsAdmin(projectId, userId, true));
    }
}
