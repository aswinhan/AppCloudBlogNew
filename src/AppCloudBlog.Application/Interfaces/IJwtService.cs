namespace AppCloudBlog.Application.Interfaces;

public interface IJwtService
{
    Task<AuthResponseDto> GenerateJwtAndRefreshToken(ApplicationUser user);
}