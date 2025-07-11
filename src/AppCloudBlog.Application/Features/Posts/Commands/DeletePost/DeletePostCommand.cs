namespace AppCloudBlog.Application.Features.Posts.Commands.DeletePost;

// 1. Command/Request Definition
public record DeletePostCommand(Guid PostId, Guid CurrentUserId) : IRequest<Unit>;

// 2. Validator for the Command
public class DeletePostCommandValidator : AbstractValidator<DeletePostCommand>
{
    public DeletePostCommandValidator()
    {
        RuleFor(x => x.PostId)
           .NotEmpty().WithMessage("Post ID is required.");
    }
}

// 3. Handler for the Command
public class DeletePostCommandHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) : IRequestHandler<DeletePostCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager = userManager; // To check user roles

    public async Task<Unit> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(request.PostId) ?? throw new NotFoundException($"Post with ID {request.PostId} not found.");

        // Authorization check: Only author or Admin can delete
        var currentUser = await _userManager.FindByIdAsync(request.CurrentUserId.ToString()) ?? throw new UnauthorizedException("Current user not found.");
        var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
        if (post.AuthorId != request.CurrentUserId && !isAdmin)
        {
            throw new ForbiddenException("You are not authorized to delete this post.");
        }

        await _unitOfWork.Posts.DeleteAsync(post);
        await _unitOfWork.CommitAsync();

        return Unit.Value;
    }
}