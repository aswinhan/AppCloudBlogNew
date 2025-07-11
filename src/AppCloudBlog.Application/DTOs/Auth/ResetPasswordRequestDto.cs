namespace AppCloudBlog.Application.DTOs.Auth;

public class ResetPasswordRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty; // Token sent via email
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}