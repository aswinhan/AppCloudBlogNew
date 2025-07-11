namespace AppCloudBlog.Domain.Entities.JoinEntities;

public class PostCategory : BaseEntity
{
    // Foreign Key to Post
    public Guid PostId { get; set; }
    public Post Post { get; set; } = default!; // Navigation property

    // Foreign Key to Category
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = default!; // Navigation property
}