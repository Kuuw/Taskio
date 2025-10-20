using Entities.DTO;
using Entities.Models;

namespace Services.Abstract;

public interface ITaskService : IGenericService<Entities.Models.Task, TaskPostDto, TaskGetDto, TaskPutDto>
{
}