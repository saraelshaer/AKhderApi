using BlogSystemApi.Consts;
using System.Linq.Expressions;

namespace AKhderApi.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync
         (
             Expression<Func<T, bool>> criteria = null,
             string[] includes = null,
             Expression<Func<T, object>> orderBy = null,
             OrderByDirection orderByDirection = OrderByDirection.Ascending,
             int pageNumber = 1,
             int pageSize = 10
         );
        Task<T> GetByIdAsync<U>( U id);
        Task AddAsync(T entity);
        void Update(T entity);
        void HardDelete(T entity);
        void SoftDelete(T entity);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate);
        Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
        Task<bool> Exists(Expression<Func<T, bool>> criteria);
        Task<int> CountAsync(Expression<Func<T, bool>> criteria = null);
        void RemoveRange(IEnumerable<T> entities);
    }

}
