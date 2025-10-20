using Data.Abstract;
using Entities.Models;

namespace Data.Concrete
{
    internal class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
    }
}
