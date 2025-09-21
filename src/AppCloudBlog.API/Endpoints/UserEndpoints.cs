namespace AppCloudBlog.API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app, ApiVersionSet apiVersionSet)
    {
        var group = app.MapGroup("/api/v{version:apiVersion}/users")
                     .WithApiVersionSet(apiVersionSet)
                     .WithTags("users");

        // GET /api/v1/users/{id}
        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetUserProfileQuery(id));
            return Results.Ok(new ApiResponse<UserDto> { Data = result });
        })
       .RequireAuthorization() // Requires authenticated user to view profiles
       .WithOpenApi();

        // PUT /api/v1/users/{id}
        group.MapPut("/{id:guid}", async (Guid id, UserProfileUpdateDto profileDto, ClaimsPrincipal user, ISender sender) =>
        {
            var currentUserId = user.GetUserId();
            if (id != currentUserId)
            {
                // Optionally, allow Admin to update any profile
                // var isAdmin = user.IsInRole("Admin");
                // if (!isAdmin) throw new ForbiddenException("You can only update your own profile.");
                throw new ForbiddenException("You can only update your own profile.");
            }
            var result = await sender.Send(new UpdateUserProfileCommand(id, profileDto));
            return Results.Ok(new ApiResponse<UserDto> { Data = result });
        })
       .RequireAuthorization()
       .WithOpenApi();

        // GET /api/v1/users (Admin only)
        group.MapGet("/", async (ISender sender) =>
        {
            var result = await sender.Send(new GetAllUsersQuery());
            return Results.Ok(new ApiResponse<IReadOnlyList<UserDto>> { Data = result });
        })
       .RequireAuthorization("ADMIN") // Requires Admin role
       .WithOpenApi();

        // PUT /api/v1/users/{id}/toggle-status (Admin only)
        group.MapPut("/{id:guid}/toggle-status", async (Guid id, bool isActive, ISender sender) =>
        {
            await sender.Send(new ToggleUserStatusCommand(id, isActive));
            return Results.NoContent(); // 204 No Content
        })
       .RequireAuthorization("ADMIN")
       .WithOpenApi();

        // DELETE /api/v1/users/{id} (Admin only)
        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeleteUserCommand(id));
            return Results.NoContent();
        })
       .RequireAuthorization("ADMIN")
       .WithOpenApi();

        // POST /api/v1/users/{id}/follow
        group.MapPost("/{id:guid}/follow", async (Guid id, ClaimsPrincipal user, ISender sender) =>
        {
            var followerId = user.GetUserId();
            await sender.Send(new FollowUserCommand(followerId, id));
            return Results.NoContent();
        })
       .RequireAuthorization()
       .WithOpenApi();

        // POST /api/v1/users/{id}/unfollow
        group.MapPost("/{id:guid}/unfollow", async (Guid id, ClaimsPrincipal user, ISender sender) =>
        {
            var followerId = user.GetUserId();
            await sender.Send(new UnfollowUserCommand(followerId, id));
            return Results.NoContent();
        })
       .RequireAuthorization()
       .WithOpenApi();

        // GET /api/v1/users/{id}/follows?getFollowers=true/false
        group.MapGet("/{id:guid}/follows", async (Guid id, bool getFollowers, ISender sender) =>
        {
            var result = await sender.Send(new GetUserFollowsQuery(id, getFollowers));
            return Results.Ok(new ApiResponse<IReadOnlyList<UserFollowDto>> { Data = result });
        })
       .RequireAuthorization()
       .WithOpenApi();
    }
}