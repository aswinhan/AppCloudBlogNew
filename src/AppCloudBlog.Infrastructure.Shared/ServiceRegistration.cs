namespace AppCloudBlog.Infrastructure.Shared;

public static class ServiceRegistration
{
    public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration _)
    {
        // Register JWT Service
        services.AddScoped<IJwtService, JwtService>();

        // Register Email Service (Placeholder for now, concrete implementation later)
        // For now, we'll use a dummy implementation or configure a real one later.
        // Example: services.AddTransient<IEmailService, MailerSendEmailService>();
        // For now, let's add a simple placeholder implementation if not already done
        services.AddTransient<IEmailService, DummyEmailService>(); // Add this line for now
    }
}

// Updated DummyEmailService to use a consistent logging message template
public class DummyEmailService(ILogger<DummyEmailService> logger) : IEmailService
{
    private readonly ILogger<DummyEmailService> _logger = logger;

    public Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        // Use a consistent logging message template
        _logger.LogInformation("Sending dummy email. To: {ToEmail}, Subject: {Subject}, Message: {HtmlMessage}", toEmail, subject, htmlMessage);
        return Task.CompletedTask;
    }
}

