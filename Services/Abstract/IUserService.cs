using Entities.DTO;
using Entities.Models;

namespace Services.Abstract;

public interface IUserService : IGenericService<User, UserRegister, UserGetDto, UserPutDto>
{
    ServiceResult<UserGetDto?> Register(UserRegister userRegister);
    ServiceResult<UserGetDto> Get();
    new ServiceResult<bool> Update(UserPutDto userDto);
}