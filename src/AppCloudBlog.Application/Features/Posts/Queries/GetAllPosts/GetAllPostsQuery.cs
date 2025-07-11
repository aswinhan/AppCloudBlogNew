namespace AppCloudBlog.Application.Features.Posts.Queries.GetAllPosts;

// 1. Query Definition
public record GetAllPostsQuery : IRequest<IReadOnlyList<PostListDto>>;

// 2. Validator for the Query (No input to validate for now)

// 3. Handler for the Query
public class GetAllPostsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetAllPostsQuery, IReadOnlyList<PostListDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<IReadOnlyList<PostListDto>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        // Fetch all published posts with necessary details for list display
        // We need to ensure GetPublishedPostsAsync includes Author, Categories, Tags, Comments, Likes for counts
        var posts = await _unitOfWork.Posts.GetPublishedPostsAsync();

        // To ensure related data is loaded for DTO mapping, we might need a more specific repository method
        // or perform explicit loading here. For now, let's assume GetPublishedPostsAsync is sufficient
        // or we'll adjust it.
        // Let's adjust GetPublishedPostsAsync in IPostRepository and PostRepository to include necessary data.

        var postListDtos = new List<PostListDto>();
        foreach (var post in posts)
        {
            var postDto = _mapper.Map<PostListDto>(post);
            postDto.CommentCount = post.Comments?.Count ?? 0;
            postDto.LikeCount = post.Likes?.Count ?? 0;
            postListDtos.Add(postDto);
        }

        return postListDtos;
    }
}