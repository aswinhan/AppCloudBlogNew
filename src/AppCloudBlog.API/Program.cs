using AppCloudBlog.Domain.Entities;
using AppCloudBlog.Infrastructure.Persistence.Seeders;
using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add Clean Architecture Layers
builder.Services.AddSwaggerDocumentation();                           // Add Swagger documentation
builder.Services.AddCustomApiVersioning();                            // Add API Versioning configuration
builder.Services.AddApplicationServices();                            // Add application services
builder.Services.AddPersistenceInfrastructure(builder.Configuration); // Add persistence infrastructure services
builder.Services.AddSharedInfrastructure(builder.Configuration);      // Add shared infrastructure services
builder.Services.AddCustomAuthorizationPolicies();                    // Add custom authorization policies
builder.Services.AddJwtAuthentication(builder.Configuration);         // Configure JWT Authentication

builder.Services.AddSingleton<HtmlSanitizer>();                       // Register HtmlSanitizer for XSS prevention
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();       // Add global exception handler
builder.Services.AddProblemDetails();


var app = builder.Build();


var apiVersionSet = app.NewApiVersionSet() // centralized ApiVersionSet
    .HasApiVersion(1, 0)
    .ReportApiVersions()
    .Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints(apiVersionSet);
app.MapUserEndpoints(apiVersionSet);
app.MapPostEndpoints(apiVersionSet);
app.MapCategoryEndpoints(apiVersionSet);
app.MapTagEndpoints(apiVersionSet);

// Seed initial roles into the database
//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
//    await IdentitySeeder.SeedRolesAsync(roleManager);
//}

app.Run();