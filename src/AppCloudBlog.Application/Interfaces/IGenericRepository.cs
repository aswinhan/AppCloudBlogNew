namespace AppCloudBlog.Application.Interfaces;

// IGenericRepository defines common data access operations for any entity that inherits from BaseEntity.
// This helps reduce redundancy and ensures consistency across repositories. [1]
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<int> CountAsync();
    // Add more common methods as needed, e.g., for filtering:
     Task<IReadOnlyList<T>> GetWhereAsync(Expression<Func<T, bool>> predicate);
}