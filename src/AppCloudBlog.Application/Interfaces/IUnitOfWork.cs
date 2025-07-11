namespace AppCloudBlog.Application.Interfaces
{
    // IUnitOfWork coordinates multiple repositories and manages database transactions.
    // It ensures that all changes within a business operation are committed or rolled back atomically. [2, 3]
    public interface IUnitOfWork : IDisposable
    {
        // Properties for each specific repository
        IPostRepository Posts { get; }
        ICategoryRepository Categories { get; }
        ITagRepository Tags { get; }
        ICommentRepository Comments { get; }
        INotificationRepository Notifications { get; }
        IUserRepository Users { get; } // Custom user repository for specific queries

        // Method to save all changes made within the unit of work
        Task<int> CommitAsync();

        // Optional: Method to get a generic repository if needed, though specific ones are preferred for aggregates
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    }
}