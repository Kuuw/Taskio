using Entities.DTO;
using Microsoft.AspNetCore.Mvc;
using Services.Abstract;
using Services.Concrete;

namespace API.Controllers;

public class UserController : BaseController
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return HandleServiceResult(_userService.Get());
    }

    [HttpPut]
    public IActionResult UpdateUser([FromBody] UserPutDto data)
    {
        return HandleServiceResult(_userService.Update(data));
    }
}
