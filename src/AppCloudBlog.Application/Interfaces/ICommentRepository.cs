namespace AppCloudBlog.Application.Interfaces;

// ICommentRepository extends IGenericRepository for Comment-specific operations.
public interface ICommentRepository : IGenericRepository<Comment>
{
    // Add Comment-specific methods here, e.g., to get comments for a specific post, or unapproved comments
    Task<IReadOnlyList<Comment>> GetCommentsByPostIdAsync(Guid postId);
    Task<IReadOnlyList<Comment>> GetUnapprovedCommentsAsync();
}