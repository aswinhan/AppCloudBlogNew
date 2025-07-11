namespace AppCloudBlog.Domain.Entities.JoinEntities;

public class PostTag : BaseEntity
{
    // Foreign Key to Post
    public Guid PostId { get; set; }
    public Post Post { get; set; } = default!; // Navigation property

    // Foreign Key to Tag
    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = default!; // Navigation property
}