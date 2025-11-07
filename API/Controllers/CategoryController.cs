using API.Controllers;
using Entities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstract;

namespace PL.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class CategoryController : BaseController
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> CategoryGet(Guid id)
    {
        return HandleServiceResult(await _categoryService.GetByIdAsync(id));
    }

    [HttpGet("FromProjectId/{id}")]
    public async Task<IActionResult> GetCategoriesFromProject(Guid id)
    {
        return HandleServiceResult(await _categoryService.GetFromProjectAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CategoryPost([FromBody] CategoryPostDto data)
    {
        return HandleServiceResult(await _categoryService.InsertAsync(data));
    }

    [HttpPut]
    public async Task<IActionResult> CategoryPut([FromBody] CategoryPutDto data)
    {
        return HandleServiceResult(await _categoryService.UpdateAsync(data));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CategoryDelete(Guid id)
    {
        return HandleServiceResult(await _categoryService.DeleteAsync(id));
    }
}
