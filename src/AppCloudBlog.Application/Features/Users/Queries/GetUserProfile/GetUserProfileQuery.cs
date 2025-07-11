namespace AppCloudBlog.Application.Features.Users.Queries.GetUserProfile;

// 1. Query Definition
public record GetUserProfileQuery(Guid UserId) : IRequest<UserDto>;

// 2. Validator for the Query (optional, but good for basic ID validation)
public class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
{
    public GetUserProfileQueryValidator()
    {
        RuleFor(x => x.UserId)
          .NotEmpty().WithMessage("User ID is required.");
    }
}

// 3. Handler for the Query
public class GetUserProfileQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper) : IRequestHandler<GetUserProfileQuery, UserDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IMapper _mapper = mapper;

    public async Task<UserDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetUserWithDetailsAsync(request.UserId); // Use custom repo method
        if (user == null || !user.IsActive)
        {
            throw new NotFoundException("User not found or inactive.");
        }

        var userDto = _mapper.Map<UserDto>(user);

        // Manually populate roles as they are not directly mapped by Mapster from ApplicationUser
        var roles = await _userManager.GetRolesAsync(user);
        userDto.Roles = roles.ToList();

        return userDto;
    }
}