using Entities.Models;

namespace Services.Abstract;

public interface IGenericService<TModel, TPostDto, TGetDto, TPutDto>
{
    Task<ServiceResult<bool>> InsertAsync(TPostDto data);
    Task<ServiceResult<TGetDto?>> GetByIdAsync(Guid id);
    Task<ServiceResult<bool>> DeleteAsync(Guid id);
    Task<ServiceResult<bool>> UpdateAsync(TPutDto data);
    Task<ServiceResult<List<TGetDto>>> GetAllAsync();
}
