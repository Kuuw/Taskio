using Services.Abstract;
using Data.Abstract;
using Entities.DTO;
using Entities.Models;
using AutoMapper;

namespace Services.Concrete;

public class CategoryService : GenericService<Category, CategoryPostDto, CategoryGetDto, CategoryPutDto>, ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly Mapper _mapper = MapperConfig.InitializeAutomapper();

    public CategoryService(ICategoryRepository categoryRepository) : base(categoryRepository)
    {
        _categoryRepository = categoryRepository;
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