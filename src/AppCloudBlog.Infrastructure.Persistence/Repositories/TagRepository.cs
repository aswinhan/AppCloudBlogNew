namespace AppCloudBlog.Infrastructure.Persistence.Repositories;

public class TagRepository(ApplicationDbContext dbContext) : GenericRepository<Tag>(dbContext), ITagRepository
{
    public async Task<Tag?> GetBySlugAsync(string slug)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Slug == slug);
    }
}