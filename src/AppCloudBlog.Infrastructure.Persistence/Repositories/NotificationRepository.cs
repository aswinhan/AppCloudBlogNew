namespace AppCloudBlog.Infrastructure.Persistence.Repositories;

public class NotificationRepository(ApplicationDbContext dbContext) : GenericRepository<Notification>(dbContext), INotificationRepository
{
    public async Task<IReadOnlyList<Notification>> GetUserNotificationsAsync(Guid userId, bool includeRead = false)
    {
        var query = _dbSet.Where(n => n.UserId == userId);

        if (!includeRead)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query.OrderByDescending(n => n.SentDate).ToListAsync();
    }

    public async Task MarkNotificationAsReadAsync(Guid notificationId)
    {
        var notification = await _dbSet.FindAsync(notificationId);
        if (notification != null && !notification.IsRead)
        {
            notification.IsRead = true;
            _dbSet.Update(notification);
            await _dbContext.SaveChangesAsync();
        }
    }
}