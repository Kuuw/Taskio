using Data.Abstract;
using Entities.Models;

namespace Data.Concrete
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
    }
}
