using AutoMapper;
using Data.Abstract;
using Entities.Context.Abstract;
using Entities.DTO;
using Entities.Models;
using Services.Abstract;

namespace Services.Concrete;

public class CategoryService : GenericService<Category, CategoryPostDto, CategoryGetDto, CategoryPutDto>, ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository;
    protected readonly IUserContext _userContext;

    public CategoryService(ICategoryRepository categoryRepository, IProjectRepository projectRepository, ITaskRepository taskRepository, IUserContext userContext, IMapper mapper) 
        : base(categoryRepository, userContext, mapper)
    {
        _categoryRepository = categoryRepository;
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _userContext = userContext;
    }

    public async Task<ServiceResult<List<CategoryGetDto>>> GetFromProjectAsync(Guid id)
    {
        var validationResult = await ValidateCategoryAccessAsync(id, "get");
        if (validationResult != null)
        {
            return ServiceResult<List<CategoryGetDto>>.Unauthorized(validationResult.ErrorMessage ?? "Unauthorized access to categories.");
        }
        var categories = await _categoryRepository.WhereAsync(new List<Func<Category, bool>> { x => x.ProjectId == id });
        var categoryDtos = _mapper.Map<List<CategoryGetDto>>(categories);

        return ServiceResult<List<CategoryGetDto>>.Ok(categoryDtos);
    }

    public new async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return ServiceResult<bool>.NotFound("Category not found");
        }
        
        var validationResult = await ValidateCategoryAccessAsync(id, "delete");
        if (validationResult != null)
        {
            return validationResult;
        }

        var tasksToDelete = category.Tasks.ToList();
        foreach (var task in tasksToDelete)
        {
            await _taskRepository.DeleteAsync(task);
        }

        return await base.DeleteAsync(id);
    }

    public new async Task<ServiceResult<CategoryGetDto>> UpdateAsync(CategoryPutDto data)
    {
        var validationResult = await ValidateCategoryAccessAsync(data.CategoryId, "update");
        if (validationResult != null)
        {
            return ServiceResult<CategoryGetDto>.Unauthorized(validationResult.ErrorMessage ?? "Unauthorized");
        }

        var category = _mapper.Map<Category>(data);
        var result = await _categoryRepository.UpdateAsync(category);

        if (result)
        {
            var updatedCategory = await _categoryRepository.GetByIdAsync(data.CategoryId);
            if (updatedCategory != null)
            {
                var categoryDto = _mapper.Map<CategoryGetDto>(updatedCategory);
                return ServiceResult<CategoryGetDto>.Ok(categoryDto);
            }
        }

        return ServiceResult<CategoryGetDto>.InternalServerError("Failed to update category.");
    }

    public new async Task<ServiceResult<CategoryGetDto>> InsertAsync(CategoryPostDto data)
    {
        var project = await _projectRepository.GetByIdAsync(data.ProjectId);
        if (project == null)
        {
            return ServiceResult<CategoryGetDto>.NotFound("Project not found.");
        }
        if (!project.ProjectUsers.Select(x => x.UserId).Contains(_userContext.UserId))
        {
            return ServiceResult<CategoryGetDto>.Unauthorized("You do not have permission to create categories in this project.");
        }

        var category = _mapper.Map<Category>(data);
        var result = await _categoryRepository.InsertAsync(category);

        if (result)
        {
            var categories = await _categoryRepository.WhereAsync(new List<Func<Category, bool>> { x => x.ProjectId == data.ProjectId });
            var createdCategory = categories.OrderByDescending(c => c.CreatedAt).FirstOrDefault();

            if (createdCategory != null)
            {
                var categoryDto = _mapper.Map<CategoryGetDto>(createdCategory);
                return ServiceResult<CategoryGetDto>.Ok(categoryDto);
            }
        }

        return ServiceResult<CategoryGetDto>.InternalServerError("Failed to create category.");
    }

    private async Task<ServiceResult<bool>?> ValidateCategoryAccessAsync(Guid categoryId, string operation)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId);
        if (category == null)
        {
            return ServiceResult<bool>.NotFound("Category not found.");
        }
        if (!category.Project.ProjectUsers.Select(x => x.UserId).Contains(_userContext.UserId))
        {
            return ServiceResult<bool>.Unauthorized($"You do not have permission to {operation} this category.");
        }
        return null;
    }
}
