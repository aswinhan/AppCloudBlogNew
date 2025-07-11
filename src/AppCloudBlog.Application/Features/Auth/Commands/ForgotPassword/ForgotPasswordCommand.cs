namespace AppCloudBlog.Application.Features.Auth.Commands.ForgotPassword;

// 1. Command/Request Definition
public record ForgotPasswordCommand(ForgotPasswordRequestDto ForgotPasswordDto) : IRequest<Unit>; // Unit for no return value

// 2. Validator for the Command
public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.ForgotPasswordDto.Email)
          .NotEmpty().WithMessage("Email is required.")
          .EmailAddress().WithMessage("Invalid email format.");
    }
}

// 3. Handler for the Command
public class ForgotPasswordCommandHandler(
    UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    IConfiguration configuration) : IRequestHandler<ForgotPasswordCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IEmailService _emailService = emailService;
    private readonly IConfiguration _configuration = configuration; // To get frontend URL

    public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.ForgotPasswordDto.Email);
        if (user == null || !user.IsActive)
        {
            // Don't reveal that the user does not exist or is not active for security reasons
            return Unit.Value;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var frontendResetUrl = _configuration.GetSection("FrontendResetPasswordUrl").Value; // Correctly retrieve the string value

        if (string.IsNullOrEmpty(frontendResetUrl))
        {
            throw new InvalidOperationException("Frontend reset password URL is not configured.");
        }

        // Encode token for URL safety
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var resetLink = $"{frontendResetUrl}?email={WebUtility.UrlEncode(user.Email)}&token={encodedToken}";

        await _emailService.SendEmailAsync(
            user.Email!,
            "Reset Your Password for AppCloudBlog",
            $"Please reset your password by clicking this link: <a href='{resetLink}'>Reset Password</a>"
        );

        return Unit.Value; // Indicate success without returning data
    }
}