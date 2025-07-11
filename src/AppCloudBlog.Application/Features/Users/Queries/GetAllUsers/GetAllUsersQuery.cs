namespace AppCloudBlog.Application.Features.Users.Queries.GetAllUsers;

// 1. Query Definition
public record GetAllUsersQuery : IRequest<IReadOnlyList<UserDto>>;

// 2. Validator (optional, no input to validate)

// 3. Handler for the Query
public class GetAllUsersQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper) : IRequestHandler<GetAllUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager = userManager; // Still needed for GetRolesAsync
    private readonly IMapper _mapper = mapper;

    public async Task<IReadOnlyList<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        // Now, _unitOfWork.Users.GetAllAsync() returns IReadOnlyList<ApplicationUser>
        var users = await _unitOfWork.Users.GetAllAsync(); // S_R1, S_R2

        var userDtos = new List<UserDto>();
        foreach (var user in users) // Iterate over the fetched users
        {
            var userDto = _mapper.Map<UserDto>(user);
            var roles = await _userManager.GetRolesAsync(user); // [1, 2, 3]
            userDto.Roles = roles.ToList();
            userDtos.Add(userDto);
        }

        return userDtos;
    }
}