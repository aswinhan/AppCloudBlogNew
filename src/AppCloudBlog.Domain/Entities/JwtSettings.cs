namespace AppCloudBlog.Domain.Entities;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int TokenExpirationMinutes { get; set; }
    public int RefreshTokenExpirationDays { get; set; }
    public string FrontendResetPasswordUrl { get; set; } = string.Empty;
}

