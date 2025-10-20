using API.Controllers;
using Services.Abstract;
using Entities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public IActionResult CategoryGet(Guid id)
    {
        return HandleServiceResult(_categoryService.GetById(id));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetCategoriesFromProject(Guid id)
    {
        return HandleServiceResult(_categoryService.GetFromProject(id));
    }

    [HttpPost]
    public IActionResult CategoryPost([FromBody] CategoryPostDto data)
    {
        return HandleServiceResult(_categoryService.Insert(data));
    }

    [HttpPut]
    public IActionResult CategoryPut([FromBody] CategoryPutDto data)
    {
        return HandleServiceResult(_categoryService.Update(data));
    }

    [HttpDelete("{id}")]
    public IActionResult CategoryDelete(Guid id)
    {
        return HandleServiceResult(_categoryService.Delete(id));
    }
}