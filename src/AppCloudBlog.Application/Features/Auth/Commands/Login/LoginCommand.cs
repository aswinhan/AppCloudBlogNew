using AppCloudBlog.Application.Interfaces;

namespace AppCloudBlog.Application.Features.Auth.Commands.Login;

// 1. Command/Request Definition
public record LoginCommand(UserLoginDto LoginDto) : IRequest<AuthResponseDto>;

// 2. Validator for the Command
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.LoginDto.Email)
          .NotEmpty().WithMessage("Email is required.")
          .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.LoginDto.Password)
          .NotEmpty().WithMessage("Password is required.");
    }
}

// 3. Handler for the Command
public class LoginCommandHandler(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtService jwtService,
    IUnitOfWork unitOfWork) : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.LoginDto.Email);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedException("Invalid credentials or inactive account.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.LoginDto.Password, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        // Generate JWT and Refresh Token
        var authResponse = await _jwtService.GenerateJwtAndRefreshToken(user);

        await _unitOfWork.CommitAsync(); // Commit the refresh token creation and any other changes

        return authResponse;
    }
}