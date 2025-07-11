namespace AppCloudBlog.Application.DTOs.Tags;

public class TagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int PostCount { get; set; } // Aggregated count of posts with this tag
}