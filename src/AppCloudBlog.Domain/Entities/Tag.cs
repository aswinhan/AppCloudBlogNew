namespace AppCloudBlog.Domain.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty; // URL-friendly identifier

    // Navigation property for Many-to-Many with Post
    public ICollection<PostTag> PostTags { get; set; } = new HashSet<PostTag>();
}