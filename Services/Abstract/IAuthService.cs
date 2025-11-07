using Entities.DTO;
using Entities.Models;

namespace Services.Abstract;

public interface IAuthService
{
    Task<ServiceResult<AuthenticateResponse?>> AuthenticateAsync(UserLogin userLogin);
}
