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
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public ProjectService(IProjectRepository projectRepository, IUserRepository userRepository, IUserContext userContext, IMapper mapper) 
        : base(projectRepository, userContext, mapper)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public async Task<ServiceResult<List<ProjectGetDto>>> GetAsync()
    {
        Console.WriteLine(_userContext.UserId);
        var projects = await _projectRepository.GetFromUserIdAsync(_userContext.UserId);
        Console.WriteLine(projects.Count);
        if (projects == null || !projects.Any())
        {
            return ServiceResult<List<ProjectGetDto>>.NotFound("No projects found for the current user.");
        }

        var projectDtos = _mapper.Map<List<ProjectGetDto>>(projects);

        return ServiceResult<List<ProjectGetDto>>.Ok(projectDtos);
    }

    public async Task<ServiceResult<bool>> AddUserToProjectAsync(Guid projectId, string email)
    {
        var validationResult = await ValidateProjectAndAdminAccessAsync(projectId);
        if (!validationResult.Success)
        {
            return ServiceResult<bool>.BadRequest(validationResult.ErrorMessage ?? "Unauthorized");
        }
        
        var project = validationResult.Data!;
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return ServiceResult<bool>.NotFound("User with this email not found.");
        }
        if (project.ProjectUsers.Any(pu => pu.UserId == user.UserId))
        {
            return ServiceResult<bool>.BadRequest("User is already part of the project.");
        }
        project.ProjectUsers.Add(new ProjectUser { ProjectId = projectId, UserId = user.UserId, IsAdmin = false });
        var result = await _projectRepository.UpdateAsync(project);
        return ServiceResult<bool>.Ok(result);
    }

    public async Task<ServiceResult<bool>> RemoveUserFromProjectAsync(Guid projectId, string email)
    {
        if (email == _userContext.Email)
        {
            return ServiceResult<bool>.BadRequest("You cannot remove yourself from the project.");
        }
        var validationResult = await ValidateProjectAndAdminAccessAsync(projectId);
        if (!validationResult.Success)
        {
            return ServiceResult<bool>.BadRequest(validationResult.ErrorMessage ?? "Unauthorized");
        }
        
        var project = validationResult.Data!;
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return ServiceResult<bool>.NotFound("User with this email not found.");
        }
        var projectUser = project.ProjectUsers.FirstOrDefault(pu => pu.UserId == user.UserId);
        if (projectUser == null)
        {
            return ServiceResult<bool>.BadRequest("User is not part of the project.");
        }
        project.ProjectUsers.Remove(projectUser);
        var result = await _projectRepository.UpdateAsync(project);
        return ServiceResult<bool>.Ok(result);
    }

    public async Task<ServiceResult<bool>> SetUserAsAdminAsync(Guid projectId, Guid userId, bool isAdmin)
    {
        var validationResult = await ValidateProjectAndAdminAccessAsync(projectId);
        if (!validationResult.Success)
        {
            return ServiceResult<bool>.BadRequest(validationResult.ErrorMessage ?? "Unauthorized");
        }
        
        var project = validationResult.Data!;
        var projectUser = project.ProjectUsers.FirstOrDefault(pu => pu.UserId == userId);
        if (projectUser == null)
        {
            return ServiceResult<bool>.BadRequest("User is not part of the project.");
        }
        projectUser.IsAdmin = isAdmin;
        var result = await _projectRepository.UpdateAsync(project);
        return ServiceResult<bool>.Ok(result);
    }

    public async Task<ServiceResult<ProjectGetDto>> CreateAsync(ProjectPostDto projectDto)
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
        var result = await _projectRepository.InsertAsync(project);
        
        if (result)
        {
            // Fetch the created project to return full DTO
            var projects = await _projectRepository.GetFromUserIdAsync(_userContext.UserId);
            var createdProject = projects.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
            
            if (createdProject != null)
            {
                var createdProjectDto = _mapper.Map<ProjectGetDto>(createdProject);
                return ServiceResult<ProjectGetDto>.Ok(createdProjectDto);
            }
        }
        
        return ServiceResult<ProjectGetDto>.InternalServerError("Failed to create project.");
    }

    public override async Task<ServiceResult<bool>> UpdateAsync(ProjectPutDto projectDto)
    {
        var existingProject = await _projectRepository.GetByIdAsync(projectDto.ProjectId);
        if (existingProject == null)
        {
            return ServiceResult<bool>.NotFound("Project not found.");
        }
        var validationResult = await ValidateProjectAndAdminAccessAsync(projectDto.ProjectId);
        if (!validationResult.Success)
        {
            return ServiceResult<bool>.BadRequest(validationResult.ErrorMessage ?? "Unauthorized");
        }
        existingProject.ProjectName = projectDto.ProjectName;
        var result = await _projectRepository.UpdateAsync(existingProject);
        return ServiceResult<bool>.Ok(result);
    }

    public override async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        var validationResult = await ValidateProjectAndAdminAccessAsync(id);
        if (!validationResult.Success)
        {
            return ServiceResult<bool>.BadRequest(validationResult.ErrorMessage ?? "Unauthorized");
        }
        var result = await _projectRepository.DeleteFromIdAsync(id);
        return ServiceResult<bool>.Ok(result);
    }

    private async Task<ServiceResult<Project>> ValidateProjectAndAdminAccessAsync(Guid projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
        {
            return ServiceResult<Project>.NotFound("Project not found.");
        }
        if (project.ProjectUsers.Any(pu => pu.UserId == _userContext.UserId && !pu.IsAdmin))
        {
            return ServiceResult<Project>.Forbidden("Only project admins can manage users in the project.");
        }
        return ServiceResult<Project>.Ok(project);
    }
}
