namespace AppCloudBlog.Infrastructure.Persistence.Repositories;

public class CategoryRepository(ApplicationDbContext dbContext) : GenericRepository<Category>(dbContext), ICategoryRepository
{
    public async Task<Category?> GetBySlugAsync(string slug)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Slug == slug);
    }
}