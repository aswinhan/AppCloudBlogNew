namespace AppCloudBlog.Domain.Entities;

public class Notification : BaseEntity
{
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime SentDate { get; set; }

    // Optional: Link to the entity that triggered the notification
    public Guid? RelatedEntityId { get; set; } // e.g., PostId, CommentId

    // Foreign Key to ApplicationUser (recipient)
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = default!; // Navigation property
}