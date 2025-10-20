using Services.Abstract;
using Entities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserLogin data)
        {
            return HandleServiceResult(_authService.Authenticate(data));
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] UserRegister data)
        {
            return HandleServiceResult(_userService.Register(data));
        }
    }
}