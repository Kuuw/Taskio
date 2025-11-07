using AutoMapper;
using Services.Abstract;
using Data.Abstract;
using Entities.Models;
using Entities.Context.Abstract;

namespace Services.Concrete;

public class GenericService<TModel, TPostDto, TGetDto, TPutDto> : IGenericService<TModel, TPostDto, TGetDto, TPutDto>
    where TModel : class
{
    private readonly IGenericRepository<TModel> _repository;
    protected readonly IUserContext _userContext;
    protected readonly IMapper _mapper;

    public GenericService(IGenericRepository<TModel> repository, IUserContext userContext, IMapper mapper)
    {
        _repository = repository;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<ServiceResult<bool>> InsertAsync(TPostDto data)
    {
        var model = _mapper.Map<TModel>(data);
        return ServiceResult<bool>.Ok(await _repository.InsertAsync(model));
    }

    public async Task<ServiceResult<TGetDto?>> GetByIdAsync(Guid id)
    {
        var model = await _repository.GetByIdAsync(id);
        return ServiceResult<TGetDto?>.Ok(_mapper.Map<TGetDto?>(model));
    }

    public virtual async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        var model = await _repository.GetByIdAsync(id);
        if (model == null)
        {
            return ServiceResult<bool>.NotFound($"{typeof(TModel).Name} not found");
        }
        return ServiceResult<bool>.Ok(await _repository.DeleteAsync(model));
    }

    public virtual async Task<ServiceResult<bool>> UpdateAsync(TPutDto data)
    {
        var model = _mapper.Map<TModel>(data);
        return ServiceResult<bool>.Ok(await _repository.UpdateAsync(model));
    }

    public async Task<ServiceResult<List<TGetDto>>> GetAllAsync()
    {
        var models = _mapper.Map<List<TGetDto>>(await _repository.ListAsync());
        return ServiceResult<List<TGetDto>>.Ok(models);
    }
}
