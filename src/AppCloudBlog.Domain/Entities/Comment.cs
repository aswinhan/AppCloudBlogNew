namespace AppCloudBlog.Domain.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public DateTime CommentDate { get; set; }
    public bool IsApproved { get; set; } = false; // For moderation

    // Foreign Key to ApplicationUser (Commenter)
    public Guid CommenterId { get; set; }
    public ApplicationUser Commenter { get; set; } = default!; // Navigation property

    // Foreign Key to Post
    public Guid PostId { get; set; }
    public Post Post { get; set; } = default!; // Navigation property

    // Self-referencing for replies (Parent-Child relationship)
    public Guid? ParentCommentId { get; set; } // Null for top-level comments
    public Comment? ParentComment { get; set; } // Navigation property to parent
    public ICollection<Comment> Replies { get; set; } = new HashSet<Comment>(); // Navigation property to children
}