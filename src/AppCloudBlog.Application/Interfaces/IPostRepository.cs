namespace AppCloudBlog.Application.Interfaces;

// IPostRepository extends IGenericRepository for Post-specific operations.
public interface IPostRepository : IGenericRepository<Post>
{
    // Add Post-specific methods here, e.g., to get published posts, or posts by slug
    Task<IReadOnlyList<Post>> GetPublishedPostsAsync();
    Task<Post?> GetBySlugAsync(string slug);
    Task<IReadOnlyList<Post>> GetPostsByCategoryIdAsync(Guid categoryId);
    Task<IReadOnlyList<Post>> GetPostsByTagIdAsync(Guid tagId);
    Task<IReadOnlyList<Post>> GetPostsByAuthorIdAsync(Guid authorId);
}