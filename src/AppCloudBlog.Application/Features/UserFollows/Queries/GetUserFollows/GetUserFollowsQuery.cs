namespace AppCloudBlog.Application.Features.UserFollows.Queries.GetUserFollows;

// 1. Query Definition
public record GetUserFollowsQuery(Guid UserId, bool GetFollowers) : IRequest<IReadOnlyList<UserFollowDto>>;

// 2. Validator for the Query
public class GetUserFollowsQueryValidator : AbstractValidator<GetUserFollowsQuery>
{
    public GetUserFollowsQueryValidator()
    {
        RuleFor(x => x.UserId)
          .NotEmpty().WithMessage("User ID is required.");
    }
}

// 3. Handler for the Query
public class GetUserFollowsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetUserFollowsQuery, IReadOnlyList<UserFollowDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<IReadOnlyList<UserFollowDto>> Handle(GetUserFollowsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<ApplicationUser> users;

        if (request.GetFollowers)
        {
            users = await _unitOfWork.Users.GetUsersFollowedByAsync(request.UserId);
        }
        else
        {
            users = await _unitOfWork.Users.GetUsersFollowingAsync(request.UserId);
        }

        return _mapper.Map<IReadOnlyList<UserFollowDto>>(users);
    }
}