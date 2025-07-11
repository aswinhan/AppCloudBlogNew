namespace AppCloudBlog.Application.Features.Categories.Commands.CreateCategory;

// 1. Command/Request Definition
public record CreateCategoryCommand(CreateCategoryDto CategoryDto) : IRequest<CategoryDto>;

// 2. Validator for the Command
public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.CategoryDto.Name)
          .NotEmpty().WithMessage("Category name is required.")
          .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.")
          .MustAsync(BeUniqueCategoryName).WithMessage("The specified category name already exists.");

        RuleFor(x => x.CategoryDto.Slug)
          .NotEmpty().WithMessage("Category slug is required.")
          .MaximumLength(100).WithMessage("Category slug must not exceed 100 characters.")
          .MustAsync(BeUniqueCategorySlug).WithMessage("The specified category slug already exists.");
    }

    private async Task<bool> BeUniqueCategoryName(string name, CancellationToken _)
    {
        var existingCategory = await _unitOfWork.Categories.GetWhereAsync(c => c.Name == name);
        return existingCategory == null || !existingCategory.Any();
    }

    private async Task<bool> BeUniqueCategorySlug(string slug, CancellationToken _)
    {
        var existingCategory = await _unitOfWork.Categories.GetBySlugAsync(slug);
        return existingCategory == null;
    }
}

// 3. Handler for the Command
public class CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = _mapper.Map<Category>(request.CategoryDto);

        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.CommitAsync();

        var categoryDto = _mapper.Map<CategoryDto>(category);
        categoryDto.PostCount = 0; // New category, no posts yet

        return categoryDto;
    }
}