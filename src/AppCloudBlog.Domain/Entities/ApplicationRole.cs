namespace AppCloudBlog.Domain.Entities;

// ApplicationRole extends IdentityRole to leverage ASP.NET Core Identity's built-in features
public class ApplicationRole : IdentityRole<Guid> // Using Guid as the primary key type for IdentityRole
{
    // No additional properties needed for now, IdentityRole provides Name
}