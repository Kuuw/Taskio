using Entities.DTO;
using Entities.Models;

namespace Services.Abstract;

public interface IAuthService
{
    public ServiceResult<AuthenticateResponse?> Authenticate(UserLogin userLogin);
}