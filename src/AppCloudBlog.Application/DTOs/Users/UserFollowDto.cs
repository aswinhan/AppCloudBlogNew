namespace AppCloudBlog.Application.DTOs.Users;

public class UserFollowDto
{
    public Guid Id { get; set; } // ID of the user being followed/follower
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
}