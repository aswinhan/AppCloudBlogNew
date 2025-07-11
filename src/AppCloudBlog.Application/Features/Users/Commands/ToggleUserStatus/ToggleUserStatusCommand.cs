namespace AppCloudBlog.Application.Features.Users.Commands.ToggleUserStatus;

// 1. Command/Request Definition
public record ToggleUserStatusCommand(Guid UserId, bool IsActive) : IRequest<Unit>;

// 2. Validator for the Command
public class ToggleUserStatusCommandValidator : AbstractValidator<ToggleUserStatusCommand>
{
    public ToggleUserStatusCommandValidator()
    {
        RuleFor(x => x.UserId)
          .NotEmpty().WithMessage("User ID is required.");
    }
}

// 3. Handler for the Command
public class ToggleUserStatusCommandHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork) : IRequestHandler<ToggleUserStatusCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Unit> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundException("User not found.");
        if (user.IsActive == request.IsActive)
        {
            // No change needed
            return Unit.Value;
        }

        user.IsActive = request.IsActive;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            throw new InvalidOperationException($"Failed to update user status: {string.Join("; ", errors)}");
        }

        await _unitOfWork.CommitAsync(); // Ensure changes are saved

        return Unit.Value;
    }
}