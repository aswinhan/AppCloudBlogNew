namespace AppCloudBlog.API.Extensions;

// Extensions/AuthorizationExtensions.cs
public static class AuthorizationExtensions
{
    public static IServiceCollection AddCustomAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("PUBLISHER", policy => policy.RequireRole("PUBLISHER"))
            .AddPolicy("ADMIN", policy => policy.RequireRole("ADMIN"))
            .AddPolicy("PUBLISHER_OR_ADMIN", policy => policy.RequireRole("PUBLISHER", "ADMIN"));

        return services;
    }
}

