namespace AppCloudBlog.Application.DTOs.Comments;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CommentDate { get; set; }
    public bool IsApproved { get; set; }

    public UserDto Commenter { get; set; } = default!; // The user who made the comment

    public Guid PostId { get; set; }
    public Guid? ParentCommentId { get; set; } // Null for top-level comments

    public ICollection<CommentDto> Replies { get; set; } = new HashSet<CommentDto>(); // Nested replies
}