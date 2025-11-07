using Entities.DTO;
using Entities.Models;

namespace Services.Abstract;

public interface ICategoryService : IGenericService<Category, CategoryPostDto, CategoryGetDto, CategoryPutDto>
{
    Task<ServiceResult<List<CategoryGetDto>>> GetFromProjectAsync(Guid id);
    new Task<ServiceResult<CategoryGetDto>> InsertAsync(CategoryPostDto data);
    new Task<ServiceResult<CategoryGetDto>> UpdateAsync(CategoryPutDto data);
    new Task<ServiceResult<bool>> DeleteAsync(Guid id);
}
