using Data.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Data.Concrete
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, new()
    {
        private readonly TaskioContext _context = new TaskioContext();
        private readonly DbSet<T> data;

        public GenericRepository()
        {
            data = _context.Set<T>();
        }

        public bool Delete(T p)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                data.Remove(p);
                _context.SaveChanges();

                dbContextTransaction.Commit();
                return true;
            }
        }

        public T? GetById(Guid id)
        {
            return data.Find(id);
        }

        public bool Insert(T p)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                data.Add(p);
                _context.SaveChanges();

                dbContextTransaction.Commit();
                return true;
            }
        }

        public List<T> List()
        {
            return data.ToList();
        }

        public bool Update(T p)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                _context.SaveChanges();
                dbContextTransaction.Commit();
                return true;
            }
        }

        public List<T> Where(List<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            var query = data.AsQueryable();
            if (include != null)
            {
                query = include(query);
            }
            foreach (var pred in predicate)
            {
                query = query.Where(pred).AsQueryable();
            }
            return query.ToList();
        }

        public IQueryable<T> Queryable()
        {
            return data.AsQueryable();
        }
    }
}
