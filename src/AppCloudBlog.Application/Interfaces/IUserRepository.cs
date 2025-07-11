namespace AppCloudBlog.Application.Interfaces;

// IUserRepository provides custom query methods for ApplicationUser beyond what UserManager offers.
// It does NOT inherit from IGenericRepository<ApplicationUser> because ApplicationUser is managed by Identity.
public interface IUserRepository
{
    Task<ApplicationUser?> GetUserWithDetailsAsync(Guid userId);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<IReadOnlyList<ApplicationUser>> GetPublishersAsync();
    Task<IReadOnlyList<ApplicationUser>> GetUsersFollowingAsync(Guid userId);
    Task<IReadOnlyList<ApplicationUser>> GetUsersFollowedByAsync(Guid userId);
    Task AddUserFollowAsync(Guid followerId, Guid followingId);
    Task RemoveUserFollowAsync(Guid followerId, Guid followingId);
    Task<bool> IsFollowingAsync(Guid followerId, Guid followingId);

    // Add this method to retrieve all users, returning a list of ApplicationUser
    Task<IReadOnlyList<ApplicationUser>> GetAllAsync();
}