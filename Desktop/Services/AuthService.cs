using Entities.DTO;

namespace Desktop.Services;

public interface IAuthService
{
    Task<AuthenticateResponse?> LoginAsync(UserLogin loginData);
    Task<AuthenticateResponse?> RegisterAsync(UserRegister registerData);
    void SetToken(string token);
    void ClearToken();
}

public class AuthService : BaseApiService, IAuthService
{
    public AuthService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<AuthenticateResponse?> LoginAsync(UserLogin loginData)
    {
        var response = await PostAsync<AuthenticateResponse>("/Auth/Login", loginData);
        if (response != null)
        {
            SetAuthToken(response.Token);
        }
        return response;
    }

    public async Task<AuthenticateResponse?> RegisterAsync(UserRegister registerData)
    {
        var response = await PostAsync<AuthenticateResponse>("/Auth/Register", registerData);
        if (response != null)
        {
            SetAuthToken(response.Token);
        }
        return response;
    }

    public void SetToken(string token)
    {
        SetAuthToken(token);
    }

    public void ClearToken()
    {
        ClearAuthToken();
    }
}
