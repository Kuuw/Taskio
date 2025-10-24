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
    private readonly Mapper _mapper = MapperConfig.InitializeAutomapper();

    public GenericService(IGenericRepository<TModel> repository, IUserContext userContext)
    {
        _repository = repository;
        _userContext = userContext;
    }

    protected Guid GetCurrentUserId()
    {
        if (_userContext.UserId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }
        return _userContext.UserId;
    }

    public ServiceResult<bool> Insert(TPostDto data)
    {
        var model = _mapper.Map<TModel>(data);
        return ServiceResult<bool>.Ok(_repository.Insert(model));
    }

    public ServiceResult<TGetDto?> GetById(Guid id)
    {
        var model = _repository.GetById(id);
        return ServiceResult<TGetDto?>.Ok(_mapper.Map<TGetDto?>(model));
    }

    public ServiceResult<bool> Delete(Guid id)
    {
        var model = _repository.GetById(id);
        if (model == null)
        {
            return ServiceResult<bool>.NotFound($"{typeof(TModel).Name} not found");
        }
        return ServiceResult<bool>.Ok(_repository.Delete(model));
    }

    public ServiceResult<bool> Update(TPutDto data)
    {
        // Write data to console for debugging purposes
        Console.WriteLine($"Updating {typeof(TModel).Name} with data: {data}");
        // Write fields of data to console
        foreach (var prop in typeof(TPutDto).GetProperties())
        {
            Console.WriteLine($"{prop.Name}: {prop.GetValue(data)}");
        }

        var model = _mapper.Map<TModel>(data);
        return ServiceResult<bool>.Ok(_repository.Update(model));
    }

    public ServiceResult<List<TGetDto>> GetAll()
    {
        var models = _mapper.Map<List<TGetDto>>(_repository.List());
        return ServiceResult<List<TGetDto>>.Ok(models);
    }
}