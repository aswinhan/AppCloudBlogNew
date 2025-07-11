namespace AppCloudBlog.Application.Features.UserFollows.Commands.UnfollowUser;

// 1. Command/Request Definition
public record UnfollowUserCommand(Guid FollowerId, Guid FollowingId) : IRequest<Unit>;

// 2. Validator for the Command
public class UnfollowUserCommandValidator : AbstractValidator<UnfollowUserCommand>
{
    public UnfollowUserCommandValidator()
    {
        RuleFor(x => x.FollowerId)
          .NotEmpty().WithMessage("Follower ID is required.");

        RuleFor(x => x.FollowingId)
          .NotEmpty().WithMessage("Following ID is required.");
    }
}

// 3. Handler for the Command
public class UnfollowUserCommandHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) : IRequestHandler<UnfollowUserCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Unit> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
    {
        var follower = await _userManager.FindByIdAsync(request.FollowerId.ToString());
        var following = await _userManager.FindByIdAsync(request.FollowingId.ToString());

        if (follower == null || !follower.IsActive)
        {
            throw new NotFoundException("Follower user not found or inactive.");
        }
        if (following == null || !following.IsActive)
        {
            throw new NotFoundException("User to unfollow not found or inactive.");
        }

        var isFollowing = await _unitOfWork.Users.IsFollowingAsync(request.FollowerId, request.FollowingId);
        if (!isFollowing)
        {
            throw new NotFoundException("User is not currently following this author.");
        }

        await _unitOfWork.Users.RemoveUserFollowAsync(request.FollowerId, request.FollowingId);
        // RemoveUserFollowAsync already calls SaveChangesAsync internally.
        await _unitOfWork.CommitAsync(); // Commit the removal of UserFollow entry

        return Unit.Value;
    }
}