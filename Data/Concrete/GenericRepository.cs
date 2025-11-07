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

        public async Task<bool> DeleteAsync(T p)
        {
            await using (var dbContextTransaction = await _context.Database.BeginTransactionAsync())
            {
                data.Remove(p);
                await _context.SaveChangesAsync();

                await dbContextTransaction.CommitAsync();
                return true;
            }
        }

        public async Task<bool> DeleteFromIdAsync(Guid id)
        {
            await using (var dbContextTransaction = await _context.Database.BeginTransactionAsync())
            {
                var entity = await data.FindAsync(id);
                if (entity == null)
                {
                    return false;
                }
                data.Remove(entity);
                await _context.SaveChangesAsync();
                await dbContextTransaction.CommitAsync();
                return true;
            }
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await data.FindAsync(id);
        }

        public async Task<bool> InsertAsync(T p)
        {
            await using (var dbContextTransaction = await _context.Database.BeginTransactionAsync())
            {
                await data.AddAsync(p);
                await _context.SaveChangesAsync();

                await dbContextTransaction.CommitAsync();
                return true;
            }
        }

        public async Task<List<T>> ListAsync()
        {
            return await data.ToListAsync();
        }

        public virtual async Task<bool> UpdateAsync(T p)
        {
            await using (var dbContextTransaction = await _context.Database.BeginTransactionAsync())
            {
                var keyValues = _context.Entry(p).Properties
                    .Where(prop => prop.Metadata.IsPrimaryKey())
                    .Select(prop => prop.CurrentValue)
                    .ToArray();

                var existingEntity = keyValues.Length > 0 ? await data.FindAsync(keyValues) : null;
                
                if (existingEntity != null)
                {
                    _context.Entry(existingEntity).State = EntityState.Detached;
                }

                var entry = _context.Entry(p);
                if (entry.State == EntityState.Detached)
                {
                    data.Attach(p);
                }
                entry.State = EntityState.Modified;
                
                await _context.SaveChangesAsync();
                await dbContextTransaction.CommitAsync();
                return true;
            }
        }

        public async Task<List<T>> WhereAsync(List<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> include = null)
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
            return await Task.FromResult(query.ToList());
        }

        public IQueryable<T> Queryable()
        {
            return data.AsQueryable();
        }
    }
}
