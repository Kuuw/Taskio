using Entities.DTO;
using Entities.Models;

namespace Services.Abstract;

public interface IProjectService : IGenericService<Project, ProjectPostDto, ProjectGetDto, ProjectPutDto>
{
    Task<ServiceResult<List<ProjectGetDto>>> GetAsync();
    Task<ServiceResult<bool>> AddUserToProjectAsync(Guid projectId, string email);
    Task<ServiceResult<bool>> RemoveUserFromProjectAsync(Guid projectId, string email);
    Task<ServiceResult<bool>> SetUserAsAdminAsync(Guid projectId, Guid userId, bool isAdmin);
    Task<ServiceResult<ProjectGetDto>> CreateAsync(ProjectPostDto data);
    new Task<ServiceResult<bool>> UpdateAsync(ProjectPutDto data);
    new Task<ServiceResult<bool>> DeleteAsync(Guid id);
}
