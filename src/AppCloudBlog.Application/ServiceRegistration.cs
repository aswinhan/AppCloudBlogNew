namespace AppCloudBlog.Application;

public static class ServiceRegistration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR
        // Scans the current assembly (AppCloudBlog.Application) for MediatR handlers,
        // requests, and behaviors.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register FluentValidation
        // Scans the current assembly for validators and registers them.
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly()); // Scans current assembly for IRegister implementations
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>(); // Register Mapster's IMapper

        // Register MediatR Pipeline Behaviors
        // Order matters: Validation should typically run before logging or other side effects.
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviors.ValidationBehavior<,>));
        // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AppCloudBlog.Application.Behaviors.LoggingBehavior<,>));
        // Add other behaviors here (e.g., TransactionBehavior, CachingBehavior)
    }
}