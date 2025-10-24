using Entities.DTO;
using Entities.Models;

namespace Services.Abstract;

public interface ITaskService : IGenericService<Entities.Models.Task, TaskPostDto, TaskGetDto, TaskPutDto>
{
    public ServiceResult<TaskGetDto> Get(Guid guid);
    public ServiceResult<List<TaskGetDto>> GetTasksFromProject(Guid guid);
    public new ServiceResult<bool> Insert(TaskPostDto taskPostDto);
    public new ServiceResult<bool> Update(TaskPutDto taskPutDto);
    public new ServiceResult<bool> Delete(Guid id);
}