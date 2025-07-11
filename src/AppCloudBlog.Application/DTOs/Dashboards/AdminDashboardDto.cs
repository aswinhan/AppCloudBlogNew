namespace AppCloudBlog.Application.DTOs.Dashboards;

public class AdminDashboardDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int SuspendedUsers { get; set; }
    public int TotalPosts { get; set; }
    public int PublishedPosts { get; set; }
    public int DraftPosts { get; set; }
    public int TotalComments { get; set; }
    public int UnapprovedComments { get; set; }
    public int TotalCategories { get; set; }
    public int TotalTags { get; set; }
    public ICollection<UserDto> LatestRegistrations { get; set; } = new HashSet<UserDto>();
    public ICollection<PostListDto> LatestPosts { get; set; } = new HashSet<PostListDto>();
    public ICollection<CommentDto> LatestComments { get; set; } = new HashSet<CommentDto>();
    // Potentially add analytics data here (e.g., top posts by views, traffic sources)
}