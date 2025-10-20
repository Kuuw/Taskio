using Entities.Models;

namespace Services.Abstract;

public interface IGenericService<TModel, TPostDto, TGetDto, TPutDto>
{
    ServiceResult<bool> Insert(TPostDto data);
    ServiceResult<TGetDto?> GetById(Guid id);
    ServiceResult<bool> Delete(Guid id);
    ServiceResult<bool> Update(TPutDto data);
    ServiceResult<List<TGetDto>> GetAll();
}