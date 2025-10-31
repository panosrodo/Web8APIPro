
using Microsoft.EntityFrameworkCore;
using SchoolApp.Data;

namespace SchoolApp.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly SchoolAppDbContext context;
        protected readonly DbSet<T> dbSet;

        public BaseRepository(SchoolAppDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public virtual async Task AddAsync(T entity) => await dbSet.AddAsync(entity);


        public virtual async Task AddRangeAsync(IEnumerable<T> entities) => await dbSet.AddRangeAsync(entities);

        public virtual Task UpdateAsync(T entity)
        {
            dbSet.Attach(entity);
            //_context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            T? existingEntity = await GetAsync(id);
            if (existingEntity == null) return false;
            existingEntity.IsDeleted = true;
            existingEntity.DeletedAt = DateTime.UtcNow;
            existingEntity.ModifiedAt = DateTime.UtcNow;
            // _dbSet.Remove(existingEntity); // For hard delete
            return true;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync() => await dbSet.ToListAsync();


        public virtual async Task<T?> GetAsync(int id) => await dbSet.FindAsync(id);


        public virtual async Task<int> GetCountAsync() => await dbSet.CountAsync();
    }
}