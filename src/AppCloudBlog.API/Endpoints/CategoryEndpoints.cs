using System;

namespace AppCloudBlog.API.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app, ApiVersionSet apiVersionSet)
    {
        var group = app.MapGroup("/api/v{version:apiVersion}/categories")
                     .WithApiVersionSet(apiVersionSet)
                     .WithTags("categories");

        // POST /api/v1/categories
        group.MapPost("/", async (CreateCategoryDto categoryDto, ISender sender) =>
        {
            var result = await sender.Send(new CreateCategoryCommand(categoryDto));
            return Results.Created($"/api/v1/categories/{result.Id}", new ApiResponse<CategoryDto> { Data = result });
        })
       .RequireAuthorization("ADMIN") // Only Admins can create categories
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
       .RequireAuthorization("ADMIN")
       .WithOpenApi();

        // DELETE /api/v1/categories/{id}
        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeleteCategoryCommand(id));
            return Results.NoContent();
        })
       .RequireAuthorization("ADMIN")
       .WithOpenApi();
    }
}