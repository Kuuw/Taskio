using Entities.Models;

namespace Data.Abstract
{
    public interface IUserRepository : IGenericRepository<User>
    {
        public User? GetByEmail(string email);
    }
}
