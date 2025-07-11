namespace AppCloudBlog.Application.Features.Categories.Queries.GetAllCategories;

// 1. Query Definition
public record GetAllCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;

// 2. Validator for the Query (No input to validate for now)

// 3. Handler for the Query
public class GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetAllCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();

        var categoryDtos = new List<CategoryDto>();
        foreach (var category in categories)
        {
            var categoryDto = _mapper.Map<CategoryDto>(category);
            // Manually populate PostCount for each category
            categoryDto.PostCount = await _unitOfWork.Posts.GetPostsByCategoryIdAsync(category.Id).ContinueWith(t => t.Result.Count, cancellationToken);
            categoryDtos.Add(categoryDto);
        }

        return categoryDtos;
    }
}