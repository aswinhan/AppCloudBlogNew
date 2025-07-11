namespace AppCloudBlog.Application.DTOs.Posts;

public class UpdatePostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; // HTML content
    public string Excerpt { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? FeaturedImageUrl { get; set; }
    public bool IsPublished { get; set; }
    public ICollection<Guid> CategoryIds { get; set; } = new HashSet<Guid>();
    public ICollection<Guid> TagIds { get; set; } = new HashSet<Guid>();
}