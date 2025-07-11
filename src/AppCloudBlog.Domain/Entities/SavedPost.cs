namespace AppCloudBlog.Domain.Entities;

public class SavedPost : BaseEntity
{
    public DateTime SavedDate { get; set; }

    // Foreign Key to ApplicationUser
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = default!; // Navigation property

    // Foreign Key to Post
    public Guid PostId { get; set; }
    public Post Post { get; set; } = default!; // Navigation property
}