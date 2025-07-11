namespace AppCloudBlog.Application.DTOs.Notifications;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public DateTime SentDate { get; set; }
    public Guid? RelatedEntityId { get; set; } // e.g., PostId, CommentId
}