namespace AppCloudBlog.Application.Features.Users.Commands.UpdateUserProfile;

// 1. Command/Request Definition
public record UpdateUserProfileCommand(Guid UserId, UserProfileUpdateDto ProfileDto) : IRequest<UserDto>;

// 2. Validator for the Command
public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.ProfileDto.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.ProfileDto.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.ProfileDto.Bio)
            .MaximumLength(500).WithMessage("Bio must not exceed 500 characters.");

        RuleFor(x => x.ProfileDto.ProfilePictureUrl)
            .MaximumLength(2000).WithMessage("Profile picture URL must not exceed 2000 characters.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.ProfileDto.ProfilePictureUrl))
            .WithMessage("Invalid Profile Picture URL format.");
    }
}

// 3. Handler for the Command
public class UpdateUserProfileCommandHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper) : IRequestHandler<UpdateUserProfileCommand, UserDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IMapper _mapper = mapper;

    public async Task<UserDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null || !user.IsActive)
        {
            throw new NotFoundException("User not found or inactive.");
        }

        _mapper.Map(request.ProfileDto, user); // Map DTO properties to existing user entity

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            throw new FluentValidation.ValidationException(string.Join("; ", errors));
        }

        await _unitOfWork.CommitAsync(); // Commit changes from UserManager and potentially other tracked changes

        var updatedUserDto = _mapper.Map<UserDto>(user);
        var roles = await _userManager.GetRolesAsync(user);
        updatedUserDto.Roles = roles.ToList();

        return updatedUserDto;
    }
}