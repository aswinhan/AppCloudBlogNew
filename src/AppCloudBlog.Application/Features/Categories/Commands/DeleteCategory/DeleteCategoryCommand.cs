namespace AppCloudBlog.Application.Features.Categories.Commands.DeleteCategory;

// 1. Command/Request Definition
public record DeleteCategoryCommand(Guid Id) : IRequest<Unit>;

// 2. Validator for the Command
public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
          .NotEmpty().WithMessage("Category ID is required.");
    }
}

// 3. Handler for the Command
public class DeleteCategoryCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id) ?? throw new NotFoundException($"Category with ID {request.Id} not found.");

        // Check if there are any posts associated with this category
        var postsInCategory = await _unitOfWork.Posts.GetPostsByCategoryIdAsync(category.Id);
        if (postsInCategory.Any())
        {
            throw new ConflictException("Cannot delete category as it is associated with existing posts. Please reassign or delete associated posts first.");
        }

        await _unitOfWork.Categories.DeleteAsync(category);
        await _unitOfWork.CommitAsync();

        return Unit.Value;
    }
}