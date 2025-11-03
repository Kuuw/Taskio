using Entities.DTO;

namespace Desktop.Services;

public interface ITaskService
{
    Task<TaskGetDto?> GetByIdAsync(Guid taskId);
    Task<List<TaskGetDto>?> GetFromProjectAsync(Guid projectId);
    Task<TaskGetDto?> CreateAsync(TaskPostDto taskData);
    Task<TaskGetDto?> UpdateAsync(TaskPutDto taskData);
    Task<bool> DeleteAsync(Guid taskId);
    Task<bool> AddUserAsync(Guid taskId, string email);
    Task<bool> RemoveUserAsync(Guid taskId, string email);
}

public class TaskService : BaseApiService, ITaskService
{
    public TaskService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<TaskGetDto?> GetByIdAsync(Guid taskId)
    {
        return await GetAsync<TaskGetDto>($"/Task/{taskId}");
    }

    public async Task<List<TaskGetDto>?> GetFromProjectAsync(Guid projectId)
    {
        return await GetAsync<List<TaskGetDto>>($"/FromProjectId/{projectId}");
    }

    public async Task<TaskGetDto?> CreateAsync(TaskPostDto taskData)
    {
        return await PostAsync<TaskGetDto>("/Task", taskData);
    }

    public async Task<TaskGetDto?> UpdateAsync(TaskPutDto taskData)
    {
        return await PutAsync<TaskGetDto>("/Task", taskData);
    }

    public async Task<bool> DeleteAsync(Guid taskId)
    {
        return await base.DeleteAsync($"/Task/{taskId}");
    }

    public async Task<bool> AddUserAsync(Guid taskId, string email)
    {
        return await PutAsync<bool>($"/Task/AddUser/?taskId={taskId}&email={email}", new { });
    }

    public async Task<bool> RemoveUserAsync(Guid taskId, string email)
    {
        return await PutAsync<bool>($"/Task/RemoveUser/?taskId={taskId}&email={email}", new { });
    }
}
