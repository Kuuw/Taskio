using Services.Abstract;
using Data.Abstract;
using Entities.DTO;
using Entities.Models;

namespace Services.Concrete;

public class UserService : GenericService<User, UserRegister, UserGetDto, UserPutDto>, IUserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository) : base(userRepository)
    {
        _userRepository = userRepository;
    }
}
