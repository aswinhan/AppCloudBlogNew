namespace AppCloudBlog.Infrastructure.Persistence.UnitOfWork;

// UnitOfWork coordinates multiple repositories and manages database transactions.
// It ensures that all changes within a business operation are committed or rolled back atomically.
public class UnitOfWork(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager) : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager; // Injected for UserRepository

    // Private fields for lazy initialization of repositories
    private IPostRepository? _posts;
    private ICategoryRepository? _categories;
    private ITagRepository? _tags;
    private ICommentRepository? _comments;
    private INotificationRepository? _notifications;
    private IUserRepository? _users;
    private readonly IGenericRepository<BaseEntity>? _genericRepository; // For the generic method

    // Lazy initialization for each repository property
    public IPostRepository Posts => _posts ??= new PostRepository(_dbContext);
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_dbContext);
    public ITagRepository Tags => _tags ??= new TagRepository(_dbContext);
    public ICommentRepository Comments => _comments ??= new CommentRepository(_dbContext);
    public INotificationRepository Notifications => _notifications ??= new NotificationRepository(_dbContext);
    public IUserRepository Users => _users ??= new UserRepository(_dbContext, _userManager);

    // Generic repository method for entities not having a specific repository interface
    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        // This is a simple way to get a generic repository.
        // For more complex scenarios, you might use a factory or DI container directly.
        return _genericRepository as IGenericRepository<TEntity> ?? new GenericRepository<TEntity>(_dbContext);
    }

    public async Task<int> CommitAsync()
    {
        // Save all changes tracked by the DbContext in a single transaction.
        return await _dbContext.SaveChangesAsync();
    }

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}