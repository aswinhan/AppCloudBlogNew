namespace AppCloudBlog.Application.Features.Users.Commands.DeleteUser;

// 1. Command/Request Definition
public record DeleteUserCommand(Guid UserId) : IRequest<Unit>;

// 2. Validator for the Command
public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
          .NotEmpty().WithMessage("User ID is required.");
    }
}

// 3. Handler for the Command
public class DeleteUserCommandHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork) : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUnitOfWork _unitOfWork = unitOfWork; // To ensure related data is handled if not cascaded by EF

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundException("User not found.");

        // Important: Consider implications of deleting a user with associated data (posts, comments, etc.)
        // EF Core cascade deletes are configured in ApplicationDbContext, but review them carefully.
        // For example, if a user has posts, and you don't want to delete the posts, you'd need to
        // reassign them or prevent deletion. Our current DbContext setup uses Restrict for Posts/Comments
        // and Cascade for Likes/SavedPosts/Notifications/RefreshTokens.
        // If Restrict is hit, this operation will fail.

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            throw new InvalidOperationException($"Failed to delete user: {string.Join("; ", errors)}");
        }

        await _unitOfWork.CommitAsync(); // Ensure changes are saved

        return Unit.Value;
    }
}