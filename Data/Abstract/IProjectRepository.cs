using Entities.Models;

namespace Data.Abstract
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        Task<List<Project>> GetFromUserIdAsync(Guid userId);
    }
}
