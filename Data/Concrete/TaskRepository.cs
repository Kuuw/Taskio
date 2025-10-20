using Data.Abstract;
using Entities.Models;

namespace Data.Concrete
{
    internal class TaskRepository : GenericRepository<Entities.Models.Task>, ITaskRepository
    {
    }
}
