namespace AppCloudBlog.API.Endpoints;

public static class TagEndpoints
{
    public static void MapTagEndpoints(this WebApplication app, ApiVersionSet apiVersionSet)
    {
        var group = app.MapGroup("/api/v{version:apiVersion}/tags")
                     .WithApiVersionSet(apiVersionSet)
                     .WithTags("tags");

        // POST /api/v1/tags
        group.MapPost("/", async (CreateTagDto tagDto, ISender sender) =>
        {
            var result = await sender.Send(new CreateTagCommand(tagDto));
            return Results.Created($"/api/v1/tags/{result.Id}", new ApiResponse<TagDto> { Data = result });
        })
       .RequireAuthorization("ADMIN") // Only Admins can create tags
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
       .RequireAuthorization("ADMIN")
       .WithOpenApi();

        // DELETE /api/v1/tags/{id}
        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeleteTagCommand(id));
            return Results.NoContent();
        })
       .RequireAuthorization("ADMIN")
       .WithOpenApi();
    }
}