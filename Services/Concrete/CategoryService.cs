using Services.Abstract;
using Data.Abstract;
using Entities.DTO;
using Entities.Models;
using AutoMapper;
using Entities.Context.Abstract;

namespace Services.Concrete;

public class CategoryService : GenericService<Category, CategoryPostDto, CategoryGetDto, CategoryPutDto>, ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly Mapper _mapper = MapperConfig.InitializeAutomapper();
    protected readonly IUserContext _userContext;

    public CategoryService(ICategoryRepository categoryRepository, IUserContext userContext) : base(categoryRepository, userContext)
    {
        _categoryRepository = categoryRepository;
        _userContext = userContext;
    }

    public ServiceResult<List<CategoryGetDto>> GetFromProject(Guid id)
    {
        var categories = _categoryRepository.Where(new List<Func<Category, bool>> { x => x.ProjectId == id });
        if (categories is null || !categories.Any())
            return ServiceResult<List<CategoryGetDto>>.NotFound("No categories found for the specified project.");

        var categoryDtos = _mapper.Map<List<CategoryGetDto>>(categories);

        return ServiceResult<List<CategoryGetDto>>.Ok(categoryDtos);
    }
}