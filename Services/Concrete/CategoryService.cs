using Services.Abstract;
using Data.Abstract;
using Entities.DTO;
using Entities.Models;

namespace Services.Concrete;

public class CategoryService : GenericService<Category, CategoryPostDto, CategoryGetDto, CategoryPutDto>, ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    public CategoryService(ICategoryRepository categoryRepository) : base(categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
}