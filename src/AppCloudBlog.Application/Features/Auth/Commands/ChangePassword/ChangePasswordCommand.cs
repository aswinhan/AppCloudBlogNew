namespace AppCloudBlog.Application.Features.Auth.Commands.ChangePassword;

// 1. Command/Request Definition
public record ChangePasswordCommand(Guid UserId, ChangePasswordRequestDto ChangePasswordDto) : IRequest<Unit>;

// 2. Validator for the Command
public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.ChangePasswordDto.CurrentPassword)
          .NotEmpty().WithMessage("Current password is required.");

        RuleFor(x => x.ChangePasswordDto.NewPassword)
          .NotEmpty().WithMessage("New password is required.")
          .MinimumLength(8).WithMessage("New password must be at least 8 characters long.")
          .Matches("[A-Z]").WithMessage("New password must contain at least one uppercase letter.")
          .Matches("[a-z]").WithMessage("New password must contain at least one lowercase letter.")
          .Matches("[0-9]").WithMessage("New password must contain at least one digit.")
          .Matches("[^a-zA-Z0-9]").WithMessage("New password must contain at least one non-alphanumeric character.");

        RuleFor(x => x.ChangePasswordDto.ConfirmNewPassword)
          .Equal(x => x.ChangePasswordDto.NewPassword).WithMessage("Passwords do not match.");
    }
}

// 3. Handler for the Command
public class ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        var result = await _userManager.ChangePasswordAsync(user, request.ChangePasswordDto.CurrentPassword, request.ChangePasswordDto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            throw new FluentValidation.ValidationException(string.Join("; ", errors));
        }

        return Unit.Value;
    }
}