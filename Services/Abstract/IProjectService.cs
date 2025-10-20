using Entities.DTO;
using Entities.Models;

namespace Services.Abstract;

public interface IProjectService : IGenericService<Project, ProjectPostDto, ProjectGetDto, ProjectPutDto>
{
    public ServiceResult<List<ProjectGetDto>> Get();
}