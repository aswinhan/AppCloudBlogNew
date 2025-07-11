namespace AppCloudBlog.Application.Features.Auth.Commands.Register;

// 1. Command/Request Definition
public record RegisterUserCommand(UserRegistrationDto RegistrationDto) : IRequest<AuthResponseDto>;

// 2. Validator for the Command
public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.RegistrationDto.FirstName)
         .NotEmpty().WithMessage("First name is required.")
         .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.RegistrationDto.LastName)
         .NotEmpty().WithMessage("Last name is required.")
         .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.RegistrationDto.Email)
         .NotEmpty().WithMessage("Email is required.")
         .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.RegistrationDto.Password)
         .NotEmpty().WithMessage("Password is required.")
         .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
         .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
         .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
         .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
         .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one non-alphanumeric character.");

        RuleFor(x => x.RegistrationDto.ConfirmPassword)
         .Equal(x => x.RegistrationDto.Password).WithMessage("Passwords do not match.");
    }
}

// 3. Handler for the Command
public class RegisterUserCommandHandler(
    UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    IMapper mapper,
    IJwtService jwtService,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterUserCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IEmailService _emailService = emailService;
    private readonly IMapper _mapper = mapper;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<AuthResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.RegistrationDto.Email);
        if (existingUser != null)
        {
            throw new ConflictException("User with this email already exists.");
        }

        var user = _mapper.Map<ApplicationUser>(request.RegistrationDto);
        user.IsActive = true; // New users are active by default
        user.RegistrationDate = DateTime.UtcNow;

        var result = await _userManager.CreateAsync(user, request.RegistrationDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            throw new FluentValidation.ValidationException(string.Join("; ", errors));
        }

        // Assign default role (e.g., "Subscriber")
        await _userManager.AddToRoleAsync(user, "Subscriber");

        // Use the reusable JWT service
        var authResponse = await _jwtService.GenerateJwtAndRefreshToken(user);

        // Commit all changes (user creation, role assignment, refresh token creation)
        await _unitOfWork.CommitAsync();

        // Send confirmation email (optional, but good practice)
        // For a real application, you'd generate an email confirmation token and link
        // var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        // var confirmationLink = $"YOUR_FRONTEND_URL/confirm-email?userId={user.Id}&token={token}";
        // await _emailService.SendEmailAsync(user.Email!, "Confirm your email", $"Please confirm your account by clicking this link: {confirmationLink}");

        return authResponse;
    }
}