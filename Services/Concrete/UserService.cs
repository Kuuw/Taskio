using AutoMapper;
using Data.Abstract;
using Entities.Context.Abstract;
using Entities.DTO;
using Entities.Models;
using Services.Abstract;

namespace Services.Concrete;

public class UserService : GenericService<User, UserRegister, UserGetDto, UserPutDto>, IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IBcryptService _bcryptService;
    protected readonly IUserContext _userContext;
    private readonly Mapper _mapper = MapperConfig.InitializeAutomapper();

    public UserService(IUserRepository userRepository, IBcryptService bcryptService, IUserContext userContext) : base(userRepository, userContext)
    {
        _userRepository = userRepository;
        _bcryptService = bcryptService;
        _userContext = userContext;
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

    public ServiceResult<UserGetDto> Get()
    {
        var user = _userRepository.GetById(_userContext.UserId);
        if (user == null)
        {
            return ServiceResult<UserGetDto>.NotFound("User not found");
        }
        var userDto = _mapper.Map<UserGetDto>(user);

        return ServiceResult<UserGetDto>.Ok(userDto);
    }

    public new ServiceResult<bool> Update(UserPutDto userDto)
    {
        // Get the existing user from the database
        var existingUser = _userRepository.GetById(_userContext.UserId);

        if (existingUser == null)
        {
            return ServiceResult<bool>.NotFound("User not found");
        }

        if (!string.IsNullOrEmpty(userDto.FirstName))
        {
            existingUser.FirstName = userDto.FirstName;
        }

        if (!string.IsNullOrEmpty(userDto.LastName))
        {
            existingUser.LastName = userDto.LastName;
        }

        if (!string.IsNullOrEmpty(userDto.Email))
        {
            var emailExists = _userRepository.Where(new List<Func<User, bool>>
        {
            x => x.Email == userDto.Email && x.UserId != existingUser.UserId
        });

            if (emailExists != null && emailExists.Any())
            {
                return ServiceResult<bool>.Conflict("Email is already taken by another user");
            }

            existingUser.Email = userDto.Email;
        }

        if (!string.IsNullOrEmpty(userDto.Password))
        {
            existingUser.Password = _bcryptService.HashPassword(userDto.Password);
        }

        var updateResult = _userRepository.Update(existingUser);

        return updateResult
            ? ServiceResult<bool>.Ok(true)
            : ServiceResult<bool>.InternalServerError("Failed to update user");
    }
}
