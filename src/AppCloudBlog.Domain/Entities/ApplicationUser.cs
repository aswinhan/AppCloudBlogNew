namespace AppCloudBlog.Domain.Entities;

// ApplicationUser extends IdentityUser to leverage ASP.NET Core Identity's built-in features
public class ApplicationUser : IdentityUser<Guid> // Using Guid as the primary key type for IdentityUser
{
    // Custom properties for our blog users
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true; // For suspending/deleting accounts

    // Navigation properties for relationships
    public ICollection<Post> Posts { get; set; } = new HashSet<Post>(); // Posts authored by this user
    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>(); // Comments made by this user
    public ICollection<Like> Likes { get; set; } = new HashSet<Like>(); // Posts liked by this user
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>(); // Refresh tokens for this user
    public ICollection<SavedPost> SavedPosts { get; set; } = new HashSet<SavedPost>(); // Posts saved/bookmarked by this user
    public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>(); // Notifications for this user

    // Following/Followers (Many-to-Many relationship with self via a join entity)
    public ICollection<UserFollow> Following { get; set; } = new HashSet<UserFollow>(); // Users this user is following
    public ICollection<UserFollow> Followers { get; set; } = new HashSet<UserFollow>(); // Users who are following this user
}