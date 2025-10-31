using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Desktop.Services;

public class BaseApiService
{
    protected readonly HttpClient _httpClient;
    private string? _token;

    public BaseApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public void SetAuthToken(string token)
    {
        _token = token;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public void ClearAuthToken()
    {
        _token = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    protected async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GET Error: {ex.Message}");
            throw;
        }
    }

    protected async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"POST Error: {ex.Message}");
            throw;
        }
    }

    protected async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(endpoint, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PUT Error: {ex.Message}");
            throw;
        }
    }

    protected async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DELETE Error: {ex.Message}");
            throw;
        }
    }
}
