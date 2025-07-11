namespace AppCloudBlog.Application.Features.Tags.Queries.GetTagById;

// 1. Query Definition
public record GetTagByIdQuery(Guid Id) : IRequest<TagDto>;

// 2. Validator for the Query
public class GetTagByIdQueryValidator : AbstractValidator<GetTagByIdQuery>
{
    public GetTagByIdQueryValidator()
    {
        RuleFor(x => x.Id)
         .NotEmpty().WithMessage("Tag ID is required.");
    }
}

// 3. Handler for the Query
public class GetTagByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetTagByIdQuery, TagDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<TagDto> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(request.Id) ?? throw new NotFoundException($"Tag with ID {request.Id} not found.");
        var tagDto = _mapper.Map<TagDto>(tag);
        // Manually populate PostCount
        tagDto.PostCount = await _unitOfWork.Posts.GetPostsByTagIdAsync(tag.Id).ContinueWith(t => t.Result.Count, cancellationToken);

        return tagDto;
    }
}