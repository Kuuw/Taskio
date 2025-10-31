using Data.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Data.Concrete
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, new()
    {
        protected readonly TaskioContext _context;
        private readonly DbSet<T> data;

        public GenericRepository(TaskioContext context)
        {
            _context = context;
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

        public bool DeleteFromId(Guid id)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                var entity = data.Find(id);
                if (entity == null)
                {
                    return false;
                }
                data.Remove(entity);
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