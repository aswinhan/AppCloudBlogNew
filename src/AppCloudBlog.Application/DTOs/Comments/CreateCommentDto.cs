namespace AppCloudBlog.Application.DTOs.Comments;

public class CreateCommentDto
{
    public Guid PostId { get; set; }
    public Guid? ParentCommentId { get; set; } // Null for top-level comments
    public string Content { get; set; } = string.Empty;
}