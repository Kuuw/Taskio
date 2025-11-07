using Entities.DTO;
using Entities.Models;

namespace Services.Abstract;

public interface ITaskService : IGenericService<Entities.Models.Task, TaskPostDto, TaskGetDto, TaskPutDto>
{
    Task<ServiceResult<TaskGetDto>> GetAsync(Guid guid);
    Task<ServiceResult<List<TaskGetDto>>> GetTasksFromProjectAsync(Guid guid);
    new Task<ServiceResult<TaskGetDto>> InsertAsync(TaskPostDto taskPostDto);
    new Task<ServiceResult<TaskGetDto>> UpdateAsync(TaskPutDto taskPutDto);
    new Task<ServiceResult<bool>> DeleteAsync(Guid id);
    Task<ServiceResult<bool>> AddUserToTaskAsync(Guid taskId, string email);
    Task<ServiceResult<bool>> RemoveUserFromTaskAsync(Guid taskId, string email);
}
