namespace AppCloudBlog.API.Endpoints;

public static class TagEndpoints
{
    public static void MapTagEndpoints(this WebApplication app, ApiVersionSet apiVersionSet)
    {
        var group = app.MapGroup("/api/v{version:apiVersion}/tags")
                     .WithApiVersionSet(apiVersionSet) // Use the passed ApiVersionSet [3, 4]
                     .WithTags("tags")
                     .MapToApiVersion(1, 0); // Map to specific API version [4, 5]

        // POST /api/v1/tags
        group.MapPost("/", async (CreateTagDto tagDto, ISender sender) =>
        {
            var result = await sender.Send(new CreateTagCommand(tagDto));
            return Results.Created($"/api/v1/tags/{result.Id}", new ApiResponse<TagDto> { Data = result });
        })
       .RequireAuthorization("Admin") // Only Admins can create tags
       .WithOpenApi();

        // GET /api/v1/tags/{id}
        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetTagByIdQuery(id));
            return Results.Ok(new ApiResponse<TagDto> { Data = result });
        })
       .AllowAnonymous() // Publicly accessible
       .WithOpenApi();

        // GET /api/v1/tags
        group.MapGet("/", async (ISender sender) =>
        {
            var result = await sender.Send(new GetAllTagsQuery());
            return Results.Ok(new ApiResponse<IReadOnlyList<TagDto>> { Data = result });
        })
       .AllowAnonymous()
       .WithOpenApi();

        // PUT /api/v1/tags/{id}
        group.MapPut("/{id:guid}", async (Guid id, UpdateTagDto tagDto, ISender sender) =>
        {
            var result = await sender.Send(new UpdateTagCommand(id, tagDto));
            return Results.Ok(new ApiResponse<TagDto> { Data = result });
        })
       .RequireAuthorization("Admin")
       .WithOpenApi();

        // DELETE /api/v1/tags/{id}
        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeleteTagCommand(id));
            return Results.NoContent();
        })
       .RequireAuthorization("Admin")
       .WithOpenApi();
    }
}