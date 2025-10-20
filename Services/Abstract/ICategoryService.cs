using Entities.DTO;
using Entities.Models;

namespace Services.Abstract;

public interface ICategoryService : IGenericService<Category, CategoryPostDto, CategoryGetDto, CategoryPutDto>
{
    public ServiceResult<List<CategoryGetDto>> GetFromProject(Guid id);
}