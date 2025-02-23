using System.Linq.Expressions;

namespace PlayRank.Domain.Interfaces.Abstract
{
    public interface IRepository { }
    public interface IRepository<T> : IRepository where T : IEntity
    {
        Task<T> GetAsync(int id);

        IQueryable<T> Find(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetAll();

        Task<T> AddAsync(T entity);

        void Update(T Entity);

        Task<int> DeleteAsync(int id);

        Task<int> DeleteEntityAsync(T entity);

        Task<int> SaveChangesAsync();

        Task AddRangeAsync(IEnumerable<T> entities);

        void UpdateRange(IEnumerable<T> entities);

        Task<int> CountRecordsAsync();
    }
}
