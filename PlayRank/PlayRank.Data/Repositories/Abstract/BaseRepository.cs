using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PlayRank.Domain.Interfaces.Abstract;

namespace PlayRank.Data.Repositories.Abstract
{
    public abstract class BaseRepository : IRepository { }
    public abstract class BaseRepository<T> : BaseRepository, IRepository<T>
        where T : class, IEntity
    {
        protected readonly PlayRankContext _context;

        public BaseRepository(PlayRankContext _context)
        {
            this._context = _context;
        }
        public async Task<T> AddAsync(T entity)
        {
            await this._context.AddAsync<T>(entity);
            await this._context.SaveChangesAsync();

            return entity;
        }

        public async Task<int> DeleteAsync(int id)
        {
            await this.SetDeletedOn(id);
            return await this.SaveChangesAsync();
        }

        public async Task<int> DeleteEntityAsync(T entity)
        {
            entity.DeletedOn = DateTime.UtcNow;
            return await this.SaveChangesAsync();
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return this.GetAll().Where(predicate);
        }

        public IQueryable<T> GetAll()
        {
            return this._context.Set<T>();
        }

        public async Task<T> GetAsync(int id)
        {
            var result = await this._context.FindAsync<T>(id);

            return result;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await this._context.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            this._context.Update(entity);
            this._context.SaveChanges();
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await this._context.AddRangeAsync(entities);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            this._context.UpdateRange(entities);
        }

        public async Task<int> CountRecordsAsync()
        {
            var result = await this._context.Set<T>().CountAsync();

            return result;
        }

        private async Task SetDeletedOn(int id)
        {
            var entity = await GetAsync(id);

            entity.DeletedOn = DateTime.UtcNow;
        }
    }
}
