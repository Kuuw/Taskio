using Data.Abstract;
using Entities.Models;

namespace Data.Concrete
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
    }
}
