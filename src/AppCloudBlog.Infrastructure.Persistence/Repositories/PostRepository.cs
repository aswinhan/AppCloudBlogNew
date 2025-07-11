namespace AppCloudBlog.Infrastructure.Persistence.Repositories;

public class PostRepository(ApplicationDbContext dbContext) : GenericRepository<Post>(dbContext), IPostRepository
{
    public async Task<IReadOnlyList<Post>> GetPublishedPostsAsync()
    {
        return await _dbSet
          .Where(p => p.IsPublished)
          .Include(p => p.Author) // Include author for PostListDto
          .Include(p => p.PostCategories)
              .ThenInclude(pc => pc.Category) // Include categories
          .Include(p => p.PostTags)
              .ThenInclude(pt => pt.Tag) // Include tags
          .Include(p => p.Comments) // Include comments for count
          .Include(p => p.Likes) // Include likes for count
          .OrderByDescending(p => p.PublishDate)
          .ToListAsync();
    }


    public async Task<Post?> GetBySlugAsync(string slug)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<IReadOnlyList<Post>> GetPostsByCategoryIdAsync(Guid categoryId)
    {
        return await _dbSet
           .Where(p => p.PostCategories.Any(pc => pc.CategoryId == categoryId))
           .OrderByDescending(p => p.PublishDate)
           .ToListAsync();
    }

    public async Task<IReadOnlyList<Post>> GetPostsByTagIdAsync(Guid tagId)
    {
        return await _dbSet
           .Where(p => p.PostTags.Any(pt => pt.TagId == tagId))
           .OrderByDescending(p => p.PublishDate)
           .ToListAsync();
    }

    public async Task<IReadOnlyList<Post>> GetPostsByAuthorIdAsync(Guid authorId)
    {
        return await _dbSet
           .Where(p => p.AuthorId == authorId)
           .OrderByDescending(p => p.PublishDate)
           .ToListAsync();
    }

    public async Task<Post?> GetPostWithDetailsAsync(Guid id)
    {
        return await _dbSet
         .Include(p => p.Author)
         .Include(p => p.PostCategories)
             .ThenInclude(pc => pc.Category)
         .Include(p => p.PostTags)
             .ThenInclude(pt => pt.Tag)
         .Include(p => p.Comments) // Include top-level comments
             .ThenInclude(c => c.Commenter) // Include the commenter for top-level comments
         .Include(p => p.Comments) // Start a new Include path from comments to include their replies
             .ThenInclude(c => c.Replies) // Include the replies (which are also Comments)
                 .ThenInclude(r => r.Commenter) // Include the commenter for those replies
         .Include(p => p.Likes)
         .Include(p => p.SavedPosts)
         .FirstOrDefaultAsync(p => p.Id == id);
    }
}