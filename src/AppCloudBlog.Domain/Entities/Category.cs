namespace AppCloudBlog.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty; // URL-friendly identifier
    public string? Description { get; set; }

    // Navigation property for Many-to-Many with Post
    public ICollection<PostCategory> PostCategories { get; set; } = new HashSet<PostCategory>();
}