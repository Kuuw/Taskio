using AutoMapper;
using Data.Abstract;
using Entities.Context.Abstract;
using Entities.DTO;
using Entities.Models;
using Services.Abstract;

namespace Services.Concrete;

public class ProjectService : GenericService<Project, ProjectPostDto, ProjectGetDto, ProjectPutDto>, IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserContext _userContext;
    private readonly Mapper _mapper = MapperConfig.InitializeAutomapper();

    public ProjectService(IProjectRepository projectRepository, IUserContext userContext) : base(projectRepository)
    {
        _projectRepository = projectRepository;
        _userContext = userContext;
    }

    public ServiceResult<List<ProjectGetDto>> Get()
    {
        var projects = _projectRepository.Where(new List<Func<Project, bool>> { x => x.ProjectUsers.Select(x => x.UserId).Contains(_userContext.UserId) });

        if(projects == null || !projects.Any())
        {
            return ServiceResult<List<ProjectGetDto>>.NotFound("No projects found for the current user.");
        }

        var projectDtos = _mapper.Map<List<ProjectGetDto>>(projects);

        return ServiceResult<List<ProjectGetDto>>.Ok(projectDtos);
    }
}
