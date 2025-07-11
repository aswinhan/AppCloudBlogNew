namespace AppCloudBlog.Application.DTOs.Dashboards;

public class UserDashboardDto
{
    public UserDto UserProfile { get; set; } = default!;
    public ICollection<PostListDto> MyPosts { get; set; } = new HashSet<PostListDto>(); // Posts authored by this user
    public ICollection<PostListDto> LikedPosts { get; set; } = new HashSet<PostListDto>(); // Posts liked by this user
    public ICollection<PostListDto> SavedPosts { get; set; } = new HashSet<PostListDto>(); // Posts saved/bookmarked by this user
    public ICollection<CommentDto> MyComments { get; set; } = new HashSet<CommentDto>(); // Comments made by this user
    public ICollection<UserFollowDto> Following { get; set; } = new HashSet<UserFollowDto>(); // Users this user is following
    public ICollection<UserFollowDto> Followers { get; set; } = new HashSet<UserFollowDto>(); // Users who are following this user
    public ICollection<NotificationDto> UnreadNotifications { get; set; } = new HashSet<NotificationDto>();
    public int TotalPostViews { get; set; } // Sum of view counts for all user's posts
}