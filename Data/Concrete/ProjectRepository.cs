using Data.Abstract;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Concrete
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        private readonly TaskioContext _context;
        private readonly DbSet<Project> _project;

        public ProjectRepository(TaskioContext context): base(context)
        {
            _context = context;
            _project = _context.Set<Project>();
        }

        public List<Project> getFromUserId(Guid userId)
        {
            var projects = _project
                .Include(p => p.ProjectUsers)
                .Where(p => p.ProjectUsers.Any(pu => pu.UserId == userId))
                .ToList();
            return projects;
        }
    }
}
