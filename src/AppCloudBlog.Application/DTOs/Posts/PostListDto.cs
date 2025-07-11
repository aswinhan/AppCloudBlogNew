namespace AppCloudBlog.Application.DTOs.Posts;

public class PostListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public bool IsPublished { get; set; }
    public int ViewCount { get; set; }
    public string? FeaturedImageUrl { get; set; }

    public UserDto Author { get; set; } = default!;
    public ICollection<CategoryDto> Categories { get; set; } = new HashSet<CategoryDto>();
    public ICollection<TagDto> Tags { get; set; } = new HashSet<TagDto>();
    public int CommentCount { get; set; } // Aggregated count
    public int LikeCount { get; set; } // Aggregated count
}