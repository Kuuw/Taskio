using Data.Abstract;
using Entities.Models;

namespace Data.Concrete
{
    public class TaskRepository : GenericRepository<Entities.Models.Task>, ITaskRepository
    {
    }
}
