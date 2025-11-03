using Data.Abstract;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Concrete
{
    public class TaskRepository : GenericRepository<Entities.Models.Task>, ITaskRepository
    {
        private readonly TaskioContext _context;
        private readonly DbSet<Entities.Models.Task> _tasks;

        public TaskRepository(TaskioContext context) : base(context)
        {
            _context = context;
            _tasks = _context.Set<Entities.Models.Task>();
        }

        public override Entities.Models.Task? GetById(Guid id)
        {
            return _tasks
                .Include(t => t.Users)
                .FirstOrDefault(t => t.TaskId == id);
        }

        public override bool Update(Entities.Models.Task task)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                var existingTask = _tasks
                    .Include(t => t.Users)
                    .FirstOrDefault(t => t.TaskId == task.TaskId);

                if (existingTask == null)
                {
                    return false;
                }

                existingTask.TaskName = task.TaskName;
                existingTask.TaskDesc = task.TaskDesc;
                existingTask.DueDate = task.DueDate;
                existingTask.CategoryId = task.CategoryId;
                existingTask.UpdatedAt = task.UpdatedAt;
                existingTask.SortOrder = task.SortOrder;

                var existingUserIds = existingTask.Users.Select(u => u.UserId).ToList();
                var newUserIds = task.Users.Select(u => u.UserId).ToList();

                var usersToRemove = existingTask.Users
                    .Where(u => !newUserIds.Contains(u.UserId))
                    .ToList();

                var usersToAdd = task.Users
                    .Where(u => !existingUserIds.Contains(u.UserId))
                    .ToList();

                foreach (var userToRemove in usersToRemove)
                {
                    existingTask.Users.Remove(userToRemove);
                }

                foreach (var userToAdd in usersToAdd)
                {
                    var userEntity = _context.Set<User>().Find(userToAdd.UserId);
                    if (userEntity != null)
                    {
                        existingTask.Users.Add(userEntity);
                    }
                }

                _context.SaveChanges();
                dbContextTransaction.Commit();
                return true;
            }
        }
    }
}
