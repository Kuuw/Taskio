using Services.Abstract;
using Data.Abstract;
using Entities.DTO;
using Entities.Models;

namespace Services.Concrete;

public class ProjectService : GenericService<Project, ProjectPostDto, ProjectGetDto, ProjectPutDto>, IProjectService
{
    private readonly IProjectRepository _projectRepository;
    
    public ProjectService(IProjectRepository projectRepository) : base(projectRepository)
    {
        _projectRepository = projectRepository;
    }
}
