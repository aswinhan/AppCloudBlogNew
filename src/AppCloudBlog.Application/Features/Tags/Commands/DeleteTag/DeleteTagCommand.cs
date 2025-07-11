namespace AppCloudBlog.Application.Features.Tags.Commands.DeleteTag;

// 1. Command/Request Definition
public record DeleteTagCommand(Guid Id) : IRequest<Unit>;

// 2. Validator for the Command
public class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
{
    public DeleteTagCommandValidator()
    {
        RuleFor(x => x.Id)
         .NotEmpty().WithMessage("Tag ID is required.");
    }
}

// 3. Handler for the Command
public class DeleteTagCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteTagCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Unit> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(request.Id) ?? throw new NotFoundException($"Tag with ID {request.Id} not found.");

        // Check if there are any posts associated with this tag
        var postsWithTag = await _unitOfWork.Posts.GetPostsByTagIdAsync(tag.Id);
        if (postsWithTag.Any())
        {
            throw new ConflictException("Cannot delete tag as it is associated with existing posts. Please reassign or delete associated posts first.");
        }

        await _unitOfWork.Tags.DeleteAsync(tag);
        await _unitOfWork.CommitAsync();

        return Unit.Value;
    }
}