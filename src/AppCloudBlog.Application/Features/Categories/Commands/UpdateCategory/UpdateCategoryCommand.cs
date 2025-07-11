namespace AppCloudBlog.Application.Features.Categories.Commands.UpdateCategory;

// 1. Command/Request Definition
public record UpdateCategoryCommand(Guid Id, UpdateCategoryDto CategoryDto) : IRequest<CategoryDto>;

// 2. Validator for the Command
public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
          .NotEmpty().WithMessage("Category ID is required.");

        RuleFor(x => x.CategoryDto.Name)
          .NotEmpty().WithMessage("Category name is required.")
          .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.")
          .MustAsync(async (command, name, cancellationToken) => await BeUniqueCategoryName(command.Id, name, cancellationToken)).WithMessage("The specified category name already exists.");

        RuleFor(x => x.CategoryDto.Slug)
          .NotEmpty().WithMessage("Category slug is required.")
          .MaximumLength(100).WithMessage("Category slug must not exceed 100 characters.")
          .MustAsync(async (command, slug, cancellationToken) => await BeUniqueCategorySlug(command.Id, slug, cancellationToken)).WithMessage("The specified category slug already exists.");
    }

    private async Task<bool> BeUniqueCategoryName(Guid categoryId, string name, CancellationToken _)
    {
        var existingCategory = await _unitOfWork.Categories.GetWhereAsync(c => c.Name == name);
        return existingCategory == null || !existingCategory.Any() || existingCategory[0].Id == categoryId;
    }

    private async Task<bool> BeUniqueCategorySlug(Guid categoryId, string slug, CancellationToken _)
    {
        var existingCategory = await _unitOfWork.Categories.GetBySlugAsync(slug);
        return existingCategory == null || existingCategory.Id == categoryId;
    }
}

// 3. Handler for the Command
public class UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id) ?? throw new NotFoundException($"Category with ID {request.Id} not found.");
        _mapper.Map(request.CategoryDto, category); // Map DTO properties to existing category entity

        await _unitOfWork.Categories.UpdateAsync(category);
        await _unitOfWork.CommitAsync();

        var categoryDto = _mapper.Map<CategoryDto>(category);
        // Re-fetch post count if needed, or update manually if performance critical
        categoryDto.PostCount = await _unitOfWork.Posts.GetPostsByCategoryIdAsync(category.Id).ContinueWith(t => t.Result.Count, cancellationToken);

        return categoryDto;
    }
}