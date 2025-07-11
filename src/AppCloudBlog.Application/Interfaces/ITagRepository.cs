namespace AppCloudBlog.Application.Interfaces;

// ITagRepository extends IGenericRepository for Tag-specific operations.
public interface ITagRepository : IGenericRepository<Tag>
{
    // Add Tag-specific methods here, e.g., to get tags with post counts
    Task<Tag?> GetBySlugAsync(string slug);
}