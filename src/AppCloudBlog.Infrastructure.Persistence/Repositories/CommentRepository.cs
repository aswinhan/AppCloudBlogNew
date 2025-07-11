namespace AppCloudBlog.Infrastructure.Persistence.Repositories;

public class CommentRepository(ApplicationDbContext dbContext) : GenericRepository<Comment>(dbContext), ICommentRepository
{
    public async Task<IReadOnlyList<Comment>> GetCommentsByPostIdAsync(Guid postId)
    {
        return await _dbSet
           .Where(c => c.PostId == postId)
           .Include(c => c.Commenter) // Include commenter details
           .Include(c => c.Replies) // Include direct replies
               .ThenInclude(r => r.Commenter) // Include commenter for replies
           .OrderBy(c => c.CommentDate)
           .ToListAsync();
    }

    public async Task<IReadOnlyList<Comment>> GetUnapprovedCommentsAsync()
    {
        return await _dbSet
           .Where(c => !c.IsApproved)
           .Include(c => c.Post)
           .Include(c => c.Commenter)
           .OrderBy(c => c.CommentDate)
           .ToListAsync();
    }
}