namespace Data.Abstract
{
    public interface IGenericRepository<T>
    {
        List<T> List();
        bool Insert(T p);
        bool Delete(T p);
        public bool DeleteFromId(Guid id);
        bool Update(T p);
        T? GetById(Guid id);
        public List<T> Where(List<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> include = null);
        public IQueryable<T> Queryable();
    }
}
