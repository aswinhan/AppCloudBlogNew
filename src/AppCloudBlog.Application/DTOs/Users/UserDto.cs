namespace AppCloudBlog.Application.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime RegistrationDate { get; set; }
    public bool IsActive { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}