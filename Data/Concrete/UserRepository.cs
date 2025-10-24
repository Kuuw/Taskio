using Data.Abstract;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Concrete
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly TaskioContext _context;
        private readonly DbSet<User> _user;

        public UserRepository(TaskioContext context)
        {
            _context = context;
            _user = _context.Set<User>();
        }

        public User? GetByEmail(string email)
        {
            return _user.FirstOrDefault(u => u.Email == email);
        }
    }
}
