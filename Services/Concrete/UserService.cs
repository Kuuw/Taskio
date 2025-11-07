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

    public UserService(IUserRepository userRepository, IBcryptService bcryptService, IUserContext userContext, IMapper mapper) 
        : base(userRepository, userContext, mapper)
    {
        _userRepository = userRepository;
        _bcryptService = bcryptService;
        _userContext = userContext;
    }

    public async Task<ServiceResult<UserGetDto?>> RegisterAsync(UserRegister userRegister)
    {
        var existingUsers = await _userRepository.WhereAsync(new List<Func<User, bool>> { x => x.Email == userRegister.Email });
        if (existingUsers != null && existingUsers.Any())
        {
            return ServiceResult<UserGetDto?>.BadRequest("User with this email already exists.");
        }
        var user = new User
        {
            FirstName = userRegister.FirstName,
            LastName = userRegister.LastName,
            Email = userRegister.Email,
            Password = _bcryptService.HashPassword(userRegister.Password)
        };

        await _userRepository.InsertAsync(user);

        var newUser = await _userRepository.GetByEmailAsync(userRegister.Email);
        return ServiceResult<UserGetDto?>.Ok(_mapper.Map<UserGetDto>(newUser));
    }

    public async Task<ServiceResult<UserGetDto>> GetAsync()
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId);
        if (user == null)
        {
            return ServiceResult<UserGetDto>.NotFound("User not found");
        }
        var userDto = _mapper.Map<UserGetDto>(user);

        return ServiceResult<UserGetDto>.Ok(userDto);
    }

    public new async Task<ServiceResult<bool>> UpdateAsync(UserPutDto userDto)
    {
        // Get the existing user from the database
        var existingUser = await _userRepository.GetByIdAsync(_userContext.UserId);

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
            var emailExists = await _userRepository.WhereAsync(new List<Func<User, bool>>
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

        var updateResult = await _userRepository.UpdateAsync(existingUser);

        return updateResult
            ? ServiceResult<bool>.Ok(true)
            : ServiceResult<bool>.InternalServerError("Failed to update user");
    }
}
