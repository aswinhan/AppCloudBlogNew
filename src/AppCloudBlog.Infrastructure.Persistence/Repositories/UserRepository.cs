namespace AppCloudBlog.Infrastructure.Persistence.Repositories;

// IUserRepository does not inherit from IGenericRepository<ApplicationUser>
// because ApplicationUser is managed by ASP.NET Core Identity's UserManager.
// This repository focuses on custom queries and relationships for ApplicationUser.
public class UserRepository(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager) : IUserRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager; // Can be used for Identity-specific queries

    public async Task<ApplicationUser?> GetUserWithDetailsAsync(Guid userId)
    {
        // Example of loading a user with their posts, comments, followers, etc.
        return await _dbContext.Users
           .Include(u => u.Posts)
           .Include(u => u.Comments)
           .Include(u => u.Likes)
           .Include(u => u.SavedPosts)
           .Include(u => u.Notifications)
           .Include(u => u.Following) // Users this user is following
               .ThenInclude(uf => uf.Following) // The actual user being followed
           .Include(u => u.Followers) // Users who are following this user
               .ThenInclude(uf => uf.Follower) // The actual follower user
           .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<IReadOnlyList<ApplicationUser>> GetPublishersAsync()
    {
        // This would typically involve checking roles. For now, we'll assume any user with posts is a publisher.
        // In a real app, you'd query AspNetUserRoles table or use UserManager.GetUsersInRoleAsync("Publisher")
        return await _dbContext.Users
           .Where(u => u.Posts.Any()) // Simple heuristic for now
           .ToListAsync();
    }

    public async Task<IReadOnlyList<ApplicationUser>> GetUsersFollowingAsync(Guid userId)
    {
        return await _dbContext.UserFollows
           .Where(uf => uf.FollowerId == userId)
           .Select(uf => uf.Following)
           .ToListAsync();
    }

    public async Task<IReadOnlyList<ApplicationUser>> GetUsersFollowedByAsync(Guid userId)
    {
        return await _dbContext.UserFollows
           .Where(uf => uf.FollowingId == userId)
           .Select(uf => uf.Follower)
           .ToListAsync();
    }

    public async Task AddUserFollowAsync(Guid followerId, Guid followingId)
    {
        var userFollow = new UserFollow
        {
            FollowerId = followerId,
            FollowingId = followingId
        };
        await _dbContext.UserFollows.AddAsync(userFollow);
    }

    public async Task RemoveUserFollowAsync(Guid followerId, Guid followingId)
    {
        var userFollow = await _dbContext.UserFollows
           .FirstOrDefaultAsync(uf => uf.FollowerId == followerId && uf.FollowingId == followingId);
        if (userFollow != null)
        {
            _dbContext.UserFollows.Remove(userFollow);
        }
    }

    public async Task<bool> IsFollowingAsync(Guid followerId, Guid followingId)
    {
        return await _dbContext.UserFollows
           .AnyAsync(uf => uf.FollowerId == followerId && uf.FollowingId == followingId);
    }

    public async Task<IReadOnlyList<ApplicationUser>> GetAllAsync()
    {
        // We can directly query the DbSet<ApplicationUser> from DbContext.
        // For Identity users, UserManager.Users is also an IQueryable<ApplicationUser>.
        // Using _dbContext.Users is fine here.
        return await _dbContext.Users.ToListAsync();
    }
}