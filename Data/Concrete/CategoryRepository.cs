using Data;
using Data.Abstract;
using Data.Concrete;
using Entities.Models;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(TaskioContext context) : base(context)
    {
    }
}