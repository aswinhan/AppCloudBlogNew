namespace AppCloudBlog.Application.DTOs.Comments;

public class UpdateCommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsApproved { get; set; } // For admin moderation
}