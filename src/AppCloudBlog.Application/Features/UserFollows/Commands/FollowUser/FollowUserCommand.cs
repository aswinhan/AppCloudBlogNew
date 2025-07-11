namespace AppCloudBlog.Application.Features.UserFollows.Commands.FollowUser;

// 1. Command/Request Definition
public record FollowUserCommand(Guid FollowerId, Guid FollowingId) : IRequest<Unit>;

// 2. Validator for the Command
public class FollowUserCommandValidator : AbstractValidator<FollowUserCommand>
{
    public FollowUserCommandValidator()
    {
        RuleFor(x => x.FollowerId)
          .NotEmpty().WithMessage("Follower ID is required.");

        RuleFor(x => x.FollowingId)
          .NotEmpty().WithMessage("Following ID is required.");

        RuleFor(x => x.FollowerId)
          .NotEqual(x => x.FollowingId).WithMessage("A user cannot follow themselves.");
    }
}

// 3. Handler for the Command
public class FollowUserCommandHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) : IRequestHandler<FollowUserCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Unit> Handle(FollowUserCommand request, CancellationToken cancellationToken)
    {
        var follower = await _userManager.FindByIdAsync(request.FollowerId.ToString());
        var following = await _userManager.FindByIdAsync(request.FollowingId.ToString());

        if (follower == null || !follower.IsActive)
        {
            throw new NotFoundException("Follower user not found or inactive.");
        }
        if (following == null || !following.IsActive)
        {
            throw new NotFoundException("User to follow not found or inactive.");
        }

        var alreadyFollowing = await _unitOfWork.Users.IsFollowingAsync(request.FollowerId, request.FollowingId);
        if (alreadyFollowing)
        {
            throw new ConflictException("User is already following this author.");
        }

        await _unitOfWork.Users.AddUserFollowAsync(request.FollowerId, request.FollowingId);
        // AddUserFollowAsync already calls SaveChangesAsync internally.
        // If we want to batch operations, we'd remove SaveChangesAsync from AddUserFollowAsync
        // and call _unitOfWork.CommitAsync() here. For now, it's fine.

        // Optional: Send notification to the 'following' user
        // await _unitOfWork.Notifications.AddAsync(new Notification {
        //     UserId = request.FollowingId,
        //     Message = $"{follower.FirstName} {follower.LastName} started following you.",
        //     Type = AppCloudBlog.Domain.Enums.NotificationType.NewFollower,
        //     SentDate = DateTime.UtcNow
        // });
        // await _unitOfWork.CommitAsync();

        return Unit.Value;
    }
}