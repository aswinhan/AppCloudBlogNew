namespace AppCloudBlog.Infrastructure.Persistence;

public static class ServiceRegistration
{
    public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure DbContext with SQL Server
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Configure ASP.NET Core Identity
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
       .AddEntityFrameworkStores<ApplicationDbContext>() // Use our DbContext for Identity stores
       .AddDefaultTokenProviders(); // For password reset tokens, email confirmation tokens etc.

        // Register Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IUserRepository, UserRepository>(); // Register our custom user repository

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>(); // Specify full namespace to avoid ambiguity
    }
}