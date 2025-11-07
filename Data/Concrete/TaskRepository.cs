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

        public override async Task<Entities.Models.Task?> GetByIdAsync(Guid id)
        {
            return await _tasks
                .Include(t => t.Users)
                .FirstOrDefaultAsync(t => t.TaskId == id);
        }

        public override async Task<bool> UpdateAsync(Entities.Models.Task task)
        {
            await using (var dbContextTransaction = await _context.Database.BeginTransactionAsync())
            {
                var existingTask = await _tasks
                    .Include(t => t.Users)
                    .FirstOrDefaultAsync(t => t.TaskId == task.TaskId);

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

                if (task.Users != null && _context.Entry(task).Collection(t => t.Users).IsLoaded)
                {
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
                        var userEntity = await _context.Set<User>().FindAsync(userToAdd.UserId);
                        if (userEntity != null)
                        {
                            existingTask.Users.Add(userEntity);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await dbContextTransaction.CommitAsync();
                return true;
            }
        }
    }
}
