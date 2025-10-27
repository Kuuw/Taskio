using Services.Abstract;
using Data.Abstract;
using Entities.DTO;
using Entities.Models;
using AutoMapper;
using Entities.Context.Abstract;
using System.Linq;

namespace Services.Concrete;

public class CategoryService : GenericService<Category, CategoryPostDto, CategoryGetDto, CategoryPutDto>, ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly Mapper _mapper = MapperConfig.InitializeAutomapper();
    protected readonly IUserContext _userContext;

    public CategoryService(ICategoryRepository categoryRepository, IProjectRepository projectRepository, IUserContext userContext) : base(categoryRepository, userContext)
    {
        _categoryRepository = categoryRepository;
        _projectRepository = projectRepository;
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
        return base.Delete(id);
    }

    public new ServiceResult<bool> Update(CategoryPutDto data)
    {
        var validationResult = ValidateCategoryAccess(data.CategoryId, "update");
        if (validationResult != null)
        {
            return validationResult;
        }
        return base.Update(data);
    }

    public new ServiceResult<bool> Insert(CategoryPostDto data)
    {
        var project = _projectRepository.GetById(data.ProjectId);
        if (project == null)
        {
            return ServiceResult<bool>.NotFound("Project not found.");
        }
        if (!project.ProjectUsers.Select(x => x.UserId).Contains(_userContext.UserId))
        {
            return ServiceResult<bool>.Unauthorized("You do not have permission to create categories in this project.");
        }

        return base.Insert(data);
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