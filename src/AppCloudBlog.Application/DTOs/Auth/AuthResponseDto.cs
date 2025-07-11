namespace AppCloudBlog.Application.DTOs.Auth;

public class AuthResponseDto
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string JwtToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpires { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}