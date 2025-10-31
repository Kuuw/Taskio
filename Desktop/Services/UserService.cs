using Entities.DTO;

namespace Desktop.Services;

public interface IUserService
{
    Task<List<UserGetDto>?> GetAllAsync();
    Task<UserGetDto?> UpdateAsync(UserPutDto userData);
}

public class UserService : BaseApiService, IUserService
{
    public UserService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<List<UserGetDto>?> GetAllAsync()
    {
        return await GetAsync<List<UserGetDto>>("/User");
    }

    public async Task<UserGetDto?> UpdateAsync(UserPutDto userData)
    {
        return await PutAsync<UserGetDto>("/User", userData);
    }
}
