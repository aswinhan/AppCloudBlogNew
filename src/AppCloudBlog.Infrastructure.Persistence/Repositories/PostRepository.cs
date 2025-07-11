namespace AppCloudBlog.Infrastructure.Persistence.Repositories;

public class PostRepository(ApplicationDbContext dbContext) : GenericRepository<Post>(dbContext), IPostRepository
{
    public async Task<IReadOnlyList<Post>> GetPublishedPostsAsync()
    {
        return await _dbSet
           .Where(p => p.IsPublished)
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
}