namespace AppCloudBlog.Application.Features.Posts.Queries.GetPostById;

// 1. Query Definition
public record GetPostByIdQuery(Guid Id, Guid? CurrentUserId = null) : IRequest<PostDto>;

// 2. Validator for the Query
public class GetPostByIdQueryValidator : AbstractValidator<GetPostByIdQuery>
{
    public GetPostByIdQueryValidator()
    {
        RuleFor(x => x.Id)
           .NotEmpty().WithMessage("Post ID is required.");
    }
}

// 3. Handler for the Query
public class GetPostByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetPostByIdQuery, PostDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<PostDto> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        // Use the new method to fetch post with all necessary details
        var post = await _unitOfWork.Posts.GetPostWithDetailsAsync(request.Id) ?? throw new NotFoundException($"Post with ID {request.Id} not found."); 

        // Increment view count
        post.ViewCount++;
        await _unitOfWork.Posts.UpdateAsync(post); // Update the view count
        await _unitOfWork.CommitAsync(); // Commit the view count increment

        var postDto = _mapper.Map<PostDto>(post);

        // Manually populate aggregated counts and user-specific flags
        postDto.LikeCount = post.Likes?.Count ?? 0;
        postDto.CommentCount = post.Comments?.Count ?? 0;

        if (request.CurrentUserId.HasValue)
        {
            postDto.IsLikedByUser = post.Likes?.Any(l => l.UserId == request.CurrentUserId.Value) ?? false;
            postDto.IsSavedByUser = post.SavedPosts?.Any(sp => sp.UserId == request.CurrentUserId.Value) ?? false;
        }

        return postDto;
    }
}