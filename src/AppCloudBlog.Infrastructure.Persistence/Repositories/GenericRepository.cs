namespace AppCloudBlog.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<IReadOnlyList<T>> GetWhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
    }
}