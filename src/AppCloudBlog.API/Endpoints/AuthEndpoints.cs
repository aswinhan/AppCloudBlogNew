namespace AppCloudBlog.API.Endpoints;

public static class AuthEndpoints
{
    // Accept ApiVersionSet as a parameter [3, 4]
    public static void MapAuthEndpoints(this WebApplication app, ApiVersionSet apiVersionSet)
    {
        var group = app.MapGroup("/api/v{version:apiVersion}/auth")
                     .WithApiVersionSet(apiVersionSet) // Use the passed ApiVersionSet [3, 4]
                     .WithTags("Auth")
                     .MapToApiVersion(1, 0); // Map to specific API version [4, 5]

        // POST /api/v1/auth/login
        group.MapPost("/login", async (UserLoginDto loginDto, ISender sender) =>
        {
            var result = await sender.Send(new LoginCommand(loginDto)); // S_R1, S_R12
            return Results.Ok(new ApiResponse<AuthResponseDto> { Data = result });
        })
      .AllowAnonymous()
      .WithOpenApi();

        // POST /api/v1/auth/register
        group.MapPost("/register", async (UserRegistrationDto registrationDto, ISender sender) =>
        {
            var result = await sender.Send(new RegisterUserCommand(registrationDto));
            return Results.Created($"/api/v1/auth/login", new ApiResponse<AuthResponseDto> { Data = result });
        })
      .AllowAnonymous()
      .WithOpenApi();

        // POST /api/v1/auth/refresh-token
        group.MapPost("/refresh-token", async (RefreshTokenRequestDto refreshTokenDto, ISender sender) =>
        {
            var result = await sender.Send(new RefreshTokenCommand(refreshTokenDto));
            return Results.Ok(new ApiResponse<AuthResponseDto> { Data = result });
        })
      .AllowAnonymous()
      .WithOpenApi();

        // POST /api/v1/auth/forgot-password
        group.MapPost("/forgot-password", async (ForgotPasswordRequestDto forgotPasswordDto, ISender sender) =>
        {
            await sender.Send(new ForgotPasswordCommand(forgotPasswordDto));
            return Results.Ok(new ApiResponse<object> { Data = new { Message = "Password reset link sent if email exists." } });
        })
      .AllowAnonymous()
      .WithOpenApi();

        // POST /api/v1/auth/reset-password
        group.MapPost("/reset-password", async (ResetPasswordRequestDto resetPasswordDto, ISender sender) =>
        {
            await sender.Send(new ResetPasswordCommand(resetPasswordDto));
            return Results.Ok(new ApiResponse<object> { Data = new { Message = "Password has been reset successfully." } });
        })
      .AllowAnonymous()
      .WithOpenApi();

        // POST /api/v1/auth/change-password
        group.MapPost("/change-password", async (ClaimsPrincipal user, ChangePasswordRequestDto changePasswordDto, ISender sender) =>
        {
            var userId = user.GetUserId();
            await sender.Send(new ChangePasswordCommand(userId, changePasswordDto));
            return Results.Ok(new ApiResponse<object> { Data = new { Message = "Password has been changed successfully." } });
        })
      .RequireAuthorization()
      .WithOpenApi();
    }
}