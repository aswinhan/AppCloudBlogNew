namespace AppCloudBlog.API.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app, ApiVersionSet apiVersionSet)
    {
        var group = app.MapGroup("/api/v{version:apiVersion}/categories")
                     .WithApiVersionSet(apiVersionSet) // Use the passed ApiVersionSet [3, 4]
                     .WithTags("categories")
                     .MapToApiVersion(1, 0); // Map to specific API version [4, 5]

        // POST /api/v1/categories
        group.MapPost("/", async (CreateCategoryDto categoryDto, ISender sender) =>
        {
            var result = await sender.Send(new CreateCategoryCommand(categoryDto));
            return Results.Created($"/api/v1/categories/{result.Id}", new ApiResponse<CategoryDto> { Data = result });
        })
       .RequireAuthorization("Admin") // Only Admins can create categories
       .WithOpenApi();

        // GET /api/v1/categories/{id}
        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetCategoryByIdQuery(id));
            return Results.Ok(new ApiResponse<CategoryDto> { Data = result });
        })
       .AllowAnonymous() // Publicly accessible
       .WithOpenApi();

        // GET /api/v1/categories
        group.MapGet("/", async (ISender sender) =>
        {
            var result = await sender.Send(new GetAllCategoriesQuery());
            return Results.Ok(new ApiResponse<IReadOnlyList<CategoryDto>> { Data = result });
        })
       .AllowAnonymous()
       .WithOpenApi();

        // PUT /api/v1/categories/{id}
        group.MapPut("/{id:guid}", async (Guid id, UpdateCategoryDto categoryDto, ISender sender) =>
        {
            var result = await sender.Send(new UpdateCategoryCommand(id, categoryDto));
            return Results.Ok(new ApiResponse<CategoryDto> { Data = result });
        })
       .RequireAuthorization("Admin")
       .WithOpenApi();

        // DELETE /api/v1/categories/{id}
        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeleteCategoryCommand(id));
            return Results.NoContent();
        })
       .RequireAuthorization("Admin")
       .WithOpenApi();
    }
}