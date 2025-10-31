using AutoMapper;
using Data.Abstract;
using Data.Concrete;
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
    private readonly Mapper _mapper = MapperConfig.InitializeAutomapper();
    protected readonly IUserContext _userContext;

    public CategoryService(ICategoryRepository categoryRepository, IProjectRepository projectRepository, ITaskRepository taskRepository, IUserContext userContext) : base(categoryRepository, userContext)
    {
        _categoryRepository = categoryRepository;
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _userContext = userContext;
    }

    public ServiceResult<List<CategoryGetDto>> GetFromProject(Guid id)
    {
        var validationResult = ValidateCategoryAccess(id, "get");
        if (validationResult != null)
        {
            return ServiceResult<List<CategoryGetDto>>.Unauthorized(validationResult.ErrorMessage ?? "Unauthorized access to categories.");
        }
        var categories = _categoryRepository.Where(new List<Func<Category, bool>> { x => x.ProjectId == id });
        var categoryDtos = _mapper.Map<List<CategoryGetDto>>(categories);

        return ServiceResult<List<CategoryGetDto>>.Ok(categoryDtos);
    }

    public new ServiceResult<bool> Delete(Guid id)
    {
        var validationResult = ValidateCategoryAccess(id, "delete");
        if (validationResult != null)
        {
            return validationResult;
        }

        var category = _categoryRepository.GetById(id);
        if (category == null)
        {
            return ServiceResult<bool>.NotFound("Category not found");
        }

        var tasksToDelete = category.Tasks.ToList();
        foreach (var task in tasksToDelete)
        {
            _taskRepository.Delete(task);
        }

        return base.Delete(id);
    }

    public new ServiceResult<CategoryGetDto> Update(CategoryPutDto data)
    {
        var validationResult = ValidateCategoryAccess(data.CategoryId, "update");
        if (validationResult != null)
        {
            return ServiceResult<CategoryGetDto>.Unauthorized(validationResult.ErrorMessage ?? "Unauthorized");
        }

        var category = _mapper.Map<Category>(data);
        var result = _categoryRepository.Update(category);

        if (result)
        {
            var updatedCategory = _categoryRepository.GetById(data.CategoryId);
            if (updatedCategory != null)
            {
                var categoryDto = _mapper.Map<CategoryGetDto>(updatedCategory);
                return ServiceResult<CategoryGetDto>.Ok(categoryDto);
            }
        }

        return ServiceResult<CategoryGetDto>.InternalServerError("Failed to update category.");
    }

    public new ServiceResult<CategoryGetDto> Insert(CategoryPostDto data)
    {
        var project = _projectRepository.GetById(data.ProjectId);
        if (project == null)
        {
            return ServiceResult<CategoryGetDto>.NotFound("Project not found.");
        }
        if (!project.ProjectUsers.Select(x => x.UserId).Contains(_userContext.UserId))
        {
            return ServiceResult<CategoryGetDto>.Unauthorized("You do not have permission to create categories in this project.");
        }

        var category = _mapper.Map<Category>(data);
        var result = _categoryRepository.Insert(category);

        if (result)
        {
            var categories = _categoryRepository.Where(new List<Func<Category, bool>> { x => x.ProjectId == data.ProjectId });
            var createdCategory = categories.OrderByDescending(c => c.CreatedAt).FirstOrDefault();

            if (createdCategory != null)
            {
                var categoryDto = _mapper.Map<CategoryGetDto>(createdCategory);
                return ServiceResult<CategoryGetDto>.Ok(categoryDto);
            }
        }

        return ServiceResult<CategoryGetDto>.InternalServerError("Failed to create category.");
    }

    private ServiceResult<bool>? ValidateCategoryAccess(Guid categoryId, string operation)
    {
        var category = _categoryRepository.GetById(categoryId);
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