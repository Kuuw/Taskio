using Entities.DTO;

namespace Desktop.Services;

public interface IProjectService
{
    Task<List<ProjectGetDto>?> GetAllAsync();
    Task<ProjectGetDto?> CreateAsync(ProjectPostDto projectData);
    Task<ProjectGetDto?> UpdateAsync(ProjectPutDto projectData);
    Task<bool> DeleteAsync(Guid projectId);
    Task<bool> AddUserToProjectAsync(Guid projectId, Guid userId);
    Task<bool> RemoveUserFromProjectAsync(Guid projectId, Guid userId);
    Task<bool> MakeUserAdminAsync(Guid projectId, Guid userId);
}

public class ProjectService : BaseApiService, IProjectService
{
    public ProjectService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<List<ProjectGetDto>?> GetAllAsync()
    {
        return await GetAsync<List<ProjectGetDto>>("/Project");
    }

    public async Task<ProjectGetDto?> CreateAsync(ProjectPostDto projectData)
    {
        return await PostAsync<ProjectGetDto>("/Project", projectData);
    }

    public async Task<ProjectGetDto?> UpdateAsync(ProjectPutDto projectData)
    {
        return await PutAsync<ProjectGetDto>("/Project", projectData);
    }

    public async Task<bool> DeleteAsync(Guid projectId)
    {
        return await base.DeleteAsync($"/Project/{projectId}");
    }

    public async Task<bool> AddUserToProjectAsync(Guid projectId, String email)
    {
        return await PutAsync<bool>($"/Project/AddUser?projectId={projectId}&email={email}", new { });
    }

    public async Task<bool> RemoveUserFromProjectAsync(Guid projectId, String email)
    {
        return await PutAsync<bool>($"/Project/RemoveUser?projectId={projectId}&email={email}", new { });
    }

    public async Task<bool> MakeUserAdminAsync(Guid projectId, Guid userId)
    {
        return await PutAsync<bool>($"/Project/MakeAdmin?projectId={projectId}&userId={userId}", new { });
    }
}
