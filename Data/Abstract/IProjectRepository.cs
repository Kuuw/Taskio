using Entities.Models;

namespace Data.Abstract
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        public List<Project> getFromUserId(Guid userId);
    }
}
