namespace AppCloudBlog.Application.DTOs.Posts;

public class PostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; // HTML content
    public string Excerpt { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public bool IsPublished { get; set; }
    public int ViewCount { get; set; }
    public string? FeaturedImageUrl { get; set; }

    // Author details (nested DTO)
    public UserDto Author { get; set; } = default!;

    // Related collections (nested DTOs)
    public ICollection<CategoryDto> Categories { get; set; } = new HashSet<CategoryDto>();
    public ICollection<TagDto> Tags { get; set; } = new HashSet<TagDto>();
    public ICollection<CommentDto> Comments { get; set; } = new HashSet<CommentDto>();
    public int LikeCount { get; set; } // Aggregated count
    public bool IsLikedByUser { get; set; } // Indicates if the current user liked it
    public bool IsSavedByUser { get; set; } // Indicates if the current user saved it
    public int CommentCount { get; internal set; }
}