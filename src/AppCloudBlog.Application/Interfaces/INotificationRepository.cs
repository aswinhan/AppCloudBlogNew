namespace AppCloudBlog.Application.Interfaces
{
    // INotificationRepository extends IGenericRepository for Notification-specific operations.
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        // Add Notification-specific methods here, e.g., to get unread notifications for a user
        Task<IReadOnlyList<Notification>> GetUserNotificationsAsync(Guid userId, bool includeRead = false);
        Task MarkNotificationAsReadAsync(Guid notificationId);
    }
}