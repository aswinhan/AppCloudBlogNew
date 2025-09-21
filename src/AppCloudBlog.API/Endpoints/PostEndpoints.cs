namespace AppCloudBlog.API.Endpoints
{
    public static class PostEndpoints
    {
        public static void MapPostEndpoints(this WebApplication app, ApiVersionSet apiVersionSet)
        {
            var group = app.MapGroup("/api/v{version:apiVersion}/posts")
                     .WithApiVersionSet(apiVersionSet)
                     .WithTags("posts");

            // POST /api/v1/posts
            group.MapPost("/", async (CreatePostDto postDto, ClaimsPrincipal user, ISender sender) =>
            {
                var authorId = user.GetUserId();
                var result = await sender.Send(new CreatePostCommand(postDto, authorId)); // S_R1, S_R12
                return Results.Created($"/api/v1/posts/{result.Id}", new ApiResponse<PostDto> { Data = result });
            })
           .RequireAuthorization("PUBLISHER_OR_ADMIN") // Only Publishers or Admins can create posts
           .WithOpenApi();

            // GET /api/v1/posts/{id}
            group.MapGet("/{id:guid}", async (Guid id, ClaimsPrincipal? user, ISender sender) =>
            {
                Guid? currentUserId = null;
                if (user?.Identity?.IsAuthenticated == true)
                {
                    currentUserId = user.GetUserId();
                }
                var result = await sender.Send(new GetPostByIdQuery(id, currentUserId)); // S_R1, S_R14
                return Results.Ok(new ApiResponse<PostDto> { Data = result });
            })
           .AllowAnonymous() // Publicly accessible, but can get user-specific info if authenticated
           .WithOpenApi();

            // GET /api/v1/posts
            group.MapGet("/", async (ISender sender) =>
            {
                var result = await sender.Send(new GetAllPostsQuery()); // S_R1, S_R11, S_R22
                return Results.Ok(new ApiResponse<IReadOnlyList<PostListDto>> { Data = result });
            })
           .AllowAnonymous()
           .WithOpenApi();

            // PUT /api/v1/posts/{id}
            group.MapPut("/{id:guid}", async (Guid id, UpdatePostDto postDto, ClaimsPrincipal user, ISender sender) =>
            {
                var currentUserId = user.GetUserId();
                var result = await sender.Send(new UpdatePostCommand(id, postDto, currentUserId)); // S_R12, S_R17
                return Results.Ok(new ApiResponse<PostDto> { Data = result });
            })
           .RequireAuthorization("PUBLISHER_OR_ADMIN")
           .WithOpenApi();

            // DELETE /api/v1/posts/{id}
            group.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal user, ISender sender) =>
            {
                var currentUserId = user.GetUserId();
                await sender.Send(new DeletePostCommand(id, currentUserId)); // S_R1, S_R33, S_R47
                return Results.NoContent(); // 204 No Content
            })
           .RequireAuthorization("PUBLISHER_OR_ADMIN")
           .WithOpenApi();
        }
    }
}