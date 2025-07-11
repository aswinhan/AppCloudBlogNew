namespace AppCloudBlog.Domain.Entities;

public class Post : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; // HTML content from rich text editor
    public string Excerpt { get; set; } = string.Empty; // Short summary for display
    public string Slug { get; set; } = string.Empty; // URL-friendly identifier
    public DateTime PublishDate { get; set; }
    public bool IsPublished { get; set; }
    public int ViewCount { get; set; } = 0;
    public string? FeaturedImageUrl { get; set; } // URL to the featured image

    // Foreign Key to ApplicationUser (Author)
    public Guid AuthorId { get; set; }
    public ApplicationUser Author { get; set; } = default!; // Navigation property

    // Navigation properties for relationships
    public ICollection<PostCategory> PostCategories { get; set; } = new HashSet<PostCategory>(); // Many-to-Many with Category
    public ICollection<PostTag> PostTags { get; set; } = new HashSet<PostTag>(); // Many-to-Many with Tag
    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>(); // One-to-Many with Comment
    public ICollection<Like> Likes { get; set; } = new HashSet<Like>(); // One-to-Many with Like
    public ICollection<SavedPost> SavedPosts { get; set; } = new HashSet<SavedPost>(); // One-to-Many with SavedPost
}