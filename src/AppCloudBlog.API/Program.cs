var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add API Versioning configuration [3, 4]
builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
    config.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new QueryStringApiVersionReader("api-version"));
});

// Add Clean Architecture Layers
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceInfrastructure(builder.Configuration);
builder.Services.AddSharedInfrastructure(builder.Configuration);

// Register HtmlSanitizer for XSS prevention [1, 2]
builder.Services.AddSingleton<HtmlSanitizer>();

// Add global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
    if (jwtSettingsSection == null || !jwtSettingsSection.Exists())
    {
        throw new InvalidOperationException("JWTSettings section is not configured in appsettings.json.");
    }

    // FIX: Correctly get the string value of the "Secret" key
    var secretKey = jwtSettingsSection["Secret"]; // This was the previous error, now it should be jwtSettingsSection
    if (string.IsNullOrEmpty(secretKey))
    {
        throw new InvalidOperationException("JWT Secret key ('Secret' property) is not configured in appsettings.json.");
    }

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), // Use the string secretKey
        ValidateIssuer = true,
        ValidIssuer = jwtSettingsSection["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettingsSection["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Important for short-lived tokens [1]
    };
});

builder.Services.AddAuthorization(); // Enable authorization services

var app = builder.Build();

// Create a reusable API Version Set after app is built [3, 4]
var apiVersionSet = app.NewApiVersionSet()
   .HasApiVersion(new ApiVersion(1, 0)) // Define the API versions this set supports
   .ReportApiVersions()
   .Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(); // Register the global exception handler

app.UseHttpsRedirection();

app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization(); // Enable authorization middleware

// Map our Minimal API Endpoints, passing the apiVersionSet
app.MapAuthEndpoints(apiVersionSet);
app.MapUserEndpoints(apiVersionSet);
app.MapPostEndpoints(apiVersionSet);
app.MapCategoryEndpoints(apiVersionSet);
app.MapTagEndpoints(apiVersionSet);

app.Run();