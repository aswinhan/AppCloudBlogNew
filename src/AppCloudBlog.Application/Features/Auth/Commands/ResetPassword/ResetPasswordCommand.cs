namespace AppCloudBlog.Application.Features.Auth.Commands.ResetPassword;

// 1. Command/Request Definition
public record ResetPasswordCommand(ResetPasswordRequestDto ResetPasswordDto) : IRequest<Unit>;

// 2. Validator for the Command
public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.ResetPasswordDto.Email)
          .NotEmpty().WithMessage("Email is required.")
          .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.ResetPasswordDto.Token)
          .NotEmpty().WithMessage("Reset token is required.");

        RuleFor(x => x.ResetPasswordDto.NewPassword)
          .NotEmpty().WithMessage("New password is required.")
          .MinimumLength(8).WithMessage("New password must be at least 8 characters long.")
          .Matches("[A-Z]").WithMessage("New password must contain at least one uppercase letter.")
          .Matches("[a-z]").WithMessage("New password must contain at least one lowercase letter.")
          .Matches("[0-9]").WithMessage("New password must contain at least one digit.")
          .Matches("[^a-zA-Z0-9]").WithMessage("New password must contain at least one non-alphanumeric character.");

        RuleFor(x => x.ResetPasswordDto.ConfirmNewPassword)
          .Equal(x => x.ResetPasswordDto.NewPassword).WithMessage("Passwords do not match.");
    }
}

// 3. Handler for the Command
public class ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<ResetPasswordCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.ResetPasswordDto.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist for security reasons
            throw new UnauthorizedException("Invalid reset attempt.");
        }

        // Decode the token from URL-safe Base64
        var decodedTokenBytes = WebEncoders.Base64UrlDecode(request.ResetPasswordDto.Token);
        var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.ResetPasswordDto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            throw new FluentValidation.ValidationException(string.Join("; ", errors));
        }

        return Unit.Value;
    }
}