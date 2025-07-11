namespace AppCloudBlog.Application.Interfaces;

// ICategoryRepository extends IGenericRepository for Category-specific operations.
public interface ICategoryRepository : IGenericRepository<Category>
{
    // Add Category-specific methods here, e.g., to get categories with post counts
    Task<Category?> GetBySlugAsync(string slug);
}