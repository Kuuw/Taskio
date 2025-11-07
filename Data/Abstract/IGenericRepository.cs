namespace Data.Abstract
{
    public interface IGenericRepository<T>
    {
        Task<List<T>> ListAsync();
        Task<bool> InsertAsync(T p);
        Task<bool> DeleteAsync(T p);
        Task<bool> DeleteFromIdAsync(Guid id);
        Task<bool> UpdateAsync(T p);
        Task<T?> GetByIdAsync(Guid id);
        Task<List<T>> WhereAsync(List<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> include = null);
        IQueryable<T> Queryable();
    }
}
