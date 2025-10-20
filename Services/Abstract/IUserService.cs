using Entities.DTO;
using Entities.Models;

namespace Services.Abstract;

public interface IUserService : IGenericService<User, UserRegister, UserGetDto, UserPutDto>
{
    ServiceResult<bool> Register(UserRegister userRegister);
}