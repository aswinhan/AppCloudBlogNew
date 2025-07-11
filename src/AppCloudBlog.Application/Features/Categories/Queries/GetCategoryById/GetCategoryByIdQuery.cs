namespace AppCloudBlog.Application.Features.Categories.Queries.GetCategoryById;

// 1. Query Definition
public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;

// 2. Validator for the Query
public class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdQueryValidator()
    {
        RuleFor(x => x.Id)
          .NotEmpty().WithMessage("Category ID is required.");
    }
}

// 3. Handler for the Query
public class GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id) ?? throw new NotFoundException($"Category with ID {request.Id} not found.");
        var categoryDto = _mapper.Map<CategoryDto>(category);
        // Manually populate PostCount
        categoryDto.PostCount = await _unitOfWork.Posts.GetPostsByCategoryIdAsync(category.Id).ContinueWith(t => t.Result.Count, cancellationToken);

        return categoryDto;
    }
}