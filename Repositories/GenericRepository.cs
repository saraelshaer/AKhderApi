using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using SmartCartCarbonFootprintApi.Context;
using SmartCartCarbonFootprintApi.Repositories;
using BlogSystemApi.Consts;

namespace SmartCartCarbonFootprintApi.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(
          Expression<Func<T, bool>> criteria = null,
          string[] includes = null,
          Expression<Func<T, object>> orderBy = null,
          OrderByDirection orderByDirection = OrderByDirection.Ascending,
          int pageNumber = 1,
          int pageSize = 10)
        {
            IQueryable<T> query = _dbSet;
            if (criteria != null)
                  query = query.Where(criteria);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            if (orderBy != null)
            {
                query = (orderByDirection == OrderByDirection.Ascending)
                    ? query.OrderBy(orderBy)
                    : query.OrderByDescending(orderBy);
            }
             
            return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<T> GetByIdAsync<U>(U id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void HardDelete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void SoftDelete(T entity)
        {
            var isActiveProp = entity.GetType().GetProperty("IsActive");
            isActiveProp.SetValue(entity, false);
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        public async Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _dbSet;
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.SingleOrDefaultAsync(criteria);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> criteria = null)
        {
            IQueryable<T> query = _dbSet.AsQueryable();
            if (criteria != null)
            {
                query = query.Where(criteria);
            }
            return await query.CountAsync();
        }

        public async Task<bool> Exists(Expression<Func<T, bool>> criteria) => await _dbSet.AnyAsync(criteria);


    }
}

