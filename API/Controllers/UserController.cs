using Entities.DTO;
using Microsoft.AspNetCore.Mvc;
using Services.Abstract;

namespace API.Controllers;

public class UserController : BaseController
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return HandleServiceResult(await _userService.GetAsync());
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UserPutDto data)
    {
        return HandleServiceResult(await _userService.UpdateAsync(data));
    }
}
