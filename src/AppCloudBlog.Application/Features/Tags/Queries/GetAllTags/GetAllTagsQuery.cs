namespace AppCloudBlog.Application.Features.Tags.Queries.GetAllTags;

// 1. Query Definition
public record GetAllTagsQuery : IRequest<IReadOnlyList<TagDto>>;

// 2. Validator for the Query (No input to validate for now)

// 3. Handler for the Query
public class GetAllTagsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetAllTagsQuery, IReadOnlyList<TagDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<IReadOnlyList<TagDto>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _unitOfWork.Tags.GetAllAsync();

        var tagDtos = new List<TagDto>();
        foreach (var tag in tags)
        {
            var tagDto = _mapper.Map<TagDto>(tag);
            // Manually populate PostCount for each tag
            tagDto.PostCount = await _unitOfWork.Posts.GetPostsByTagIdAsync(tag.Id).ContinueWith(t => t.Result.Count, cancellationToken);
            tagDtos.Add(tagDto);
        }

        return tagDtos;
    }
}