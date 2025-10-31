using Entities.DTO;

namespace Desktop.Services;

public interface ICategoryService
{
    Task<CategoryGetDto?> GetByIdAsync(Guid categoryId);
    Task<List<CategoryGetDto>?> GetFromProjectAsync(Guid projectId);
    Task<CategoryGetDto?> CreateAsync(CategoryPostDto categoryData);
    Task<CategoryGetDto?> UpdateAsync(CategoryPutDto categoryData);
    Task<bool> DeleteAsync(Guid categoryId);
}

public class CategoryService : BaseApiService, ICategoryService
{
    public CategoryService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<CategoryGetDto?> GetByIdAsync(Guid categoryId)
    {
        return await GetAsync<CategoryGetDto>($"/Category/{categoryId}");
    }

    public async Task<List<CategoryGetDto>?> GetFromProjectAsync(Guid projectId)
    {
        return await GetAsync<List<CategoryGetDto>>($"/FromProjectId/{projectId}");
    }

    public async Task<CategoryGetDto?> CreateAsync(CategoryPostDto categoryData)
    {
        return await PostAsync<CategoryGetDto>("/Category", categoryData);
    }

    public async Task<CategoryGetDto?> UpdateAsync(CategoryPutDto categoryData)
    {
        return await PutAsync<CategoryGetDto>("/Category", categoryData);
    }

    public async Task<bool> DeleteAsync(Guid categoryId)
    {
        return await base.DeleteAsync($"/Category/{categoryId}");
    }
}
