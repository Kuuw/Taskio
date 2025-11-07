using Entities.DTO;
using Entities.Models;

namespace Services.Abstract;

public interface IUserService : IGenericService<User, UserRegister, UserGetDto, UserPutDto>
{
    Task<ServiceResult<UserGetDto?>> RegisterAsync(UserRegister userRegister);
    Task<ServiceResult<UserGetDto>> GetAsync();
    new Task<ServiceResult<bool>> UpdateAsync(UserPutDto userDto);
}
