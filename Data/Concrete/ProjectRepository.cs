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

        public override async Task<Project?> GetByIdAsync(Guid id)
        {
            return await _project
                .Include(p => p.ProjectUsers)
                .ThenInclude(pu => pu.User)
                .FirstOrDefaultAsync(p => p.ProjectId == id);
        }

        public override async Task<bool> UpdateAsync(Project project)
        {
            await using (var dbContextTransaction = await _context.Database.BeginTransactionAsync())
            {
                var existingProject = await _project
                    .Include(p => p.ProjectUsers)
                    .FirstOrDefaultAsync(p => p.ProjectId == project.ProjectId);

                if (existingProject == null)
                {
                    return false;
                }

                existingProject.ProjectName = project.ProjectName;
                existingProject.UpdatedAt = project.UpdatedAt;

                var existingUserIds = existingProject.ProjectUsers.Select(pu => pu.UserId).ToList();
                var newUserIds = project.ProjectUsers.Select(pu => pu.UserId).ToList();

                var usersToRemove = existingProject.ProjectUsers
                    .Where(pu => !newUserIds.Contains(pu.UserId))
                    .ToList();

                var usersToAdd = project.ProjectUsers
                    .Where(pu => !existingUserIds.Contains(pu.UserId))
                    .ToList();

                foreach (var userToRemove in usersToRemove)
                {
                    existingProject.ProjectUsers.Remove(userToRemove);
                }

                foreach (var userToAdd in usersToAdd)
                {
                    existingProject.ProjectUsers.Add(userToAdd);
                }

                var usersToUpdate = existingProject.ProjectUsers
                    .Where(pu => newUserIds.Contains(pu.UserId))
                    .ToList();

                foreach (var existingUser in usersToUpdate)
                {
                    var updatedUser = project.ProjectUsers.First(pu => pu.UserId == existingUser.UserId);
                    existingUser.IsAdmin = updatedUser.IsAdmin;
                }

                await _context.SaveChangesAsync();
                await dbContextTransaction.CommitAsync();
                return true;
            }
        }

        public async Task<List<Project>> GetFromUserIdAsync(Guid userId)
        {
            var projects = await _project
                .Include(p => p.ProjectUsers)
                    .ThenInclude(pu => pu.User)
                .Include(p => p.Categories)
                .Where(p => p.ProjectUsers.Any(pu => pu.UserId == userId))
                .ToListAsync();
            return projects;
        }
    }
}
