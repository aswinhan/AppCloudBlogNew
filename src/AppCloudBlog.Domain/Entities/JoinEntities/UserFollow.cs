namespace AppCloudBlog.Domain.Entities.JoinEntities;

public class UserFollow : BaseEntity // Inherit from BaseEntity
{
    // Foreign Key for the user who is following
    public Guid FollowerId { get; set; }
    public ApplicationUser Follower { get; set; } = default!; // Navigation property

    // Foreign Key for the user being followed (the publisher)
    public Guid FollowingId { get; set; }
    public ApplicationUser Following { get; set; } = default!; // Navigation property
}