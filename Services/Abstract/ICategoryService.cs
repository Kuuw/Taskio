using Entities.DTO;
using Entities.Models;

namespace Services.Abstract;

public interface ICategoryService : IGenericService<Category, CategoryPostDto, CategoryGetDto, CategoryPutDto>
{
    public ServiceResult<List<CategoryGetDto>> GetFromProject(Guid id);
    public new ServiceResult<CategoryGetDto> Insert(CategoryPostDto data);
    public new ServiceResult<CategoryGetDto> Update(CategoryPutDto data);
    public new ServiceResult<bool> Delete(Guid id);
}