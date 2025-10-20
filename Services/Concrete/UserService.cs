using Services.Abstract;
using Data.Abstract;
using Entities.DTO;
using Entities.Models;

namespace Services.Concrete;

public class UserService : GenericService<User, UserRegister, UserGetDto, UserPutDto>, IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IBcryptService _bcryptService;
    
    public UserService(IUserRepository userRepository, IBcryptService bcryptService) : base(userRepository)
    {
        _userRepository = userRepository;
        _bcryptService = bcryptService;
    }

    public ServiceResult<bool> Register(UserRegister userRegister)
    {
        var existingUsers = _userRepository.Where(new List<Func<User, bool>> { x => x.Email == userRegister.Email });
        if (existingUsers != null && existingUsers.Any())
        {
            return new ServiceResult<bool>
            {
                Success = false,
                ErrorMessage = "User with this email already exists.",
                Data = false
            };
        }
        var user = new User
        {
            FirstName = userRegister.FirstName,
            LastName = userRegister.LastName,
            Email = userRegister.Email,
            Password = _bcryptService.HashPassword(userRegister.Password)
        };

        _userRepository.Insert(user);
        return new ServiceResult<bool>
        {
            Success = true,
            Data = true
        };
    }
}
