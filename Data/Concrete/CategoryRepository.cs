using Data.Abstract;
using Entities.Models;

namespace Data.Concrete
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
    }
}
