namespace AppCloudBlog.Domain.Entities;

// RefreshToken must inherit from BaseEntity to be used with IGenericRepository<T>
public class RefreshToken : BaseEntity // Inherit from BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Revoked { get; set; }
    public string? ReplacedByToken { get; set; }

    // Foreign Key to ApplicationUser
    public Guid UserId { get; set; }
    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; } = default!; // Navigation property

    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsRevoked => Revoked != null;
    public bool IsActive => !IsRevoked && !IsExpired;
}