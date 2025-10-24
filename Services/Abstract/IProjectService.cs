using Entities.DTO;
using Entities.Models;

namespace Services.Abstract;

public interface IProjectService : IGenericService<Project, ProjectPostDto, ProjectGetDto, ProjectPutDto>
{
    public ServiceResult<List<ProjectGetDto>> Get();
    public ServiceResult<bool> AddUserToProject(Guid projectId, Guid userId);
    public ServiceResult<bool> RemoveUserFromProject(Guid projectId, Guid userId);
    public ServiceResult<bool> SetUserAsAdmin(Guid projectId, Guid userId, bool isAdmin);
    public ServiceResult<bool> Create(ProjectPostDto data);
    public new ServiceResult<bool> Update(ProjectPutDto data);
    public new ServiceResult<bool> Delete(Guid id);
}