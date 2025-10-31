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

    public ProjectService(IProjectRepository projectRepository, IUserContext userContext) : base(projectRepository, userContext)
    {
        _projectRepository = projectRepository;
        _userContext = userContext;
    }

    public ServiceResult<List<ProjectGetDto>> Get()
    {
        Console.WriteLine(_userContext.UserId);
        var projects = _projectRepository.getFromUserId(_userContext.UserId);
        Console.WriteLine(projects.Count);
        if (projects == null || !projects.Any())
        {
            return ServiceResult<List<ProjectGetDto>>.NotFound("No projects found for the current user.");
        }

        var projectDtos = _mapper.Map<List<ProjectGetDto>>(projects);

        return ServiceResult<List<ProjectGetDto>>.Ok(projectDtos);
    }

    public ServiceResult<bool> AddUserToProject(Guid projectId, Guid userId)
    {
        var validationResult = ValidateProjectAndAdminAccess(projectId, out var project);
        if (!validationResult.Success)
        {
            return validationResult;
        }
        if (project.ProjectUsers.Any(pu => pu.UserId == userId))
        {
            return ServiceResult<bool>.BadRequest("User is already part of the project.");
        }
        project.ProjectUsers.Add(new ProjectUser { ProjectId = projectId, UserId = userId });
        var result = _projectRepository.Update(project);
        return ServiceResult<bool>.Ok(result);
    }

    public ServiceResult<bool> RemoveUserFromProject(Guid projectId, Guid userId)
    {
        var validationResult = ValidateProjectAndAdminAccess(projectId, out var project);
        if (!validationResult.Success)
        {
            return validationResult;
        }
        var projectUser = project.ProjectUsers.FirstOrDefault(pu => pu.UserId == userId);
        if (projectUser == null)
        {
            return ServiceResult<bool>.BadRequest("User is not part of the project.");
        }
        project.ProjectUsers.Remove(projectUser);
        var result = _projectRepository.Update(project);
        return ServiceResult<bool>.Ok(result);
    }

    public ServiceResult<bool> SetUserAsAdmin(Guid projectId, Guid userId, bool isAdmin)
    {
        var validationResult = ValidateProjectAndAdminAccess(projectId, out var project);
        if (!validationResult.Success)
        {
            return validationResult;
        }
        var projectUser = project.ProjectUsers.FirstOrDefault(pu => pu.UserId == userId);
        if (projectUser == null)
        {
            return ServiceResult<bool>.BadRequest("User is not part of the project.");
        }
        projectUser.IsAdmin = isAdmin;
        var result = _projectRepository.Update(project);
        return ServiceResult<bool>.Ok(result);
    }

    public ServiceResult<ProjectGetDto> Create(ProjectPostDto projectDto)
    {
        var project = _mapper.Map<Project>(projectDto);
        project.ProjectUsers = new List<ProjectUser>
        {
            new ProjectUser
            {
                UserId = _userContext.UserId,
                IsAdmin = true
            }
        };
        var result = _projectRepository.Insert(project);
        
     if (result)
        {
       // Fetch the created project to return full DTO
            var projects = _projectRepository.getFromUserId(_userContext.UserId);
            var createdProject = projects.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
            
            if (createdProject != null)
          {
                var createdProjectDto = _mapper.Map<ProjectGetDto>(createdProject);
                return ServiceResult<ProjectGetDto>.Ok(createdProjectDto);
          }
        }
        
        return ServiceResult<ProjectGetDto>.InternalServerError("Failed to create project.");
    }

    public new ServiceResult<bool> Update(ProjectPutDto projectDto)
    {
        var existingProject = _projectRepository.GetById(projectDto.ProjectId);
        if (existingProject == null)
        {
            return ServiceResult<bool>.NotFound("Project not found.");
        }
        var validationResult = ValidateProjectAndAdminAccess(projectDto.ProjectId, out var project);
        if (!validationResult.Success)
        {
            return validationResult;
        }
        existingProject.ProjectName = projectDto.ProjectName;
        var result = _projectRepository.Update(existingProject);
        return ServiceResult<bool>.Ok(result);
    }

    public new ServiceResult<bool> Delete(Guid projectId)
    {
        var validationResult = ValidateProjectAndAdminAccess(projectId, out var project);
        if (!validationResult.Success)
        {
            return validationResult;
        }
        var result = _projectRepository.DeleteFromId(projectId);
        return ServiceResult<bool>.Ok(result);
    }

    private ServiceResult<bool> ValidateProjectAndAdminAccess(Guid projectId, out Project project)
    {
        project = _projectRepository.GetById(projectId);
        if (project == null)
        {
            return ServiceResult<bool>.NotFound("Project not found.");
        }
        if (project.ProjectUsers.Any(pu => pu.UserId == _userContext.UserId && !pu.IsAdmin))
        {
            return ServiceResult<bool>.Forbidden("Only project admins can manage users in the project.");
        }
        return ServiceResult<bool>.Ok(true);
    }
}
