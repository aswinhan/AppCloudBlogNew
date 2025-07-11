namespace AppCloudBlog.Application.Features.Posts.Commands.UpdatePost;

// 1. Command/Request Definition
public record UpdatePostCommand(Guid PostId, UpdatePostDto PostDto, Guid CurrentUserId) : IRequest<PostDto>;

// 2. Validator for the Command
public class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePostCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.PostId)
           .NotEmpty().WithMessage("Post ID is required.");

        RuleFor(x => x.PostDto.Title)
           .NotEmpty().WithMessage("Title is required.")
           .MaximumLength(250).WithMessage("Title must not exceed 250 characters.");

        RuleFor(x => x.PostDto.Content)
           .NotEmpty().WithMessage("Content is required.");

        RuleFor(x => x.PostDto.Excerpt)
           .MaximumLength(500).WithMessage("Excerpt must not exceed 500 characters.");

        RuleFor(x => x.PostDto.Slug)
           .NotEmpty().WithMessage("Slug is required.")
           .MaximumLength(250).WithMessage("Slug must not exceed 250 characters.")
           .MustAsync(async (command, slug, cancellationToken) => await BeUniqueSlug(command.PostId, slug, cancellationToken)).WithMessage("The specified slug already exists.");

        RuleFor(x => x.PostDto.FeaturedImageUrl)
           .MaximumLength(2000).WithMessage("Featured image URL must not exceed 2000 characters.")
           .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).When(x => !string.IsNullOrEmpty(x.PostDto.FeaturedImageUrl)).WithMessage("Invalid Featured Image URL format.");

        RuleFor(x => x.PostDto.CategoryIds)
           .Must(ids => ids != null && ids.Count != 0).WithMessage("At least one category is required.");
    }

    private async Task<bool> BeUniqueSlug(Guid postId, string slug, CancellationToken _)
    {
        var existingPost = await _unitOfWork.Posts.GetBySlugAsync(slug);
        return existingPost == null || existingPost.Id == postId; // Allow same slug for the same post
    }
}

// 3. Handler for the Command
public class UpdatePostCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, HtmlSanitizer htmlSanitizer, UserManager<ApplicationUser> userManager) : IRequestHandler<UpdatePostCommand, PostDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly HtmlSanitizer _htmlSanitizer = htmlSanitizer;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<PostDto> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _unitOfWork.Posts.GetPostWithDetailsAsync(request.PostId) ?? throw new NotFoundException($"Post with ID {request.PostId} not found."); // Get with details for relationships

        // Authorization check: Only author or Admin can update
        var currentUser = await _userManager.FindByIdAsync(request.CurrentUserId.ToString()) ?? throw new UnauthorizedException("Current user not found.");
        var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
        if (post.AuthorId != request.CurrentUserId && !isAdmin)
        {
            throw new ForbiddenException("You are not authorized to update this post.");
        }

        // Sanitize HTML content before mapping to entity [1]
        request.PostDto.Content = _htmlSanitizer.Sanitize(request.PostDto.Content);

        _mapper.Map(request.PostDto, post); // Map DTO properties to existing post entity

        // Update PublishDate if IsPublished status changes
        if (post.IsPublished && post.PublishDate == default)
        {
            post.PublishDate = DateTime.UtcNow;
        }
        // No need for else if (!post.IsPublished && post.PublishDate!= default) as it's not explicitly requested to clear it.

        // Update Categories (remove old, add new)
        // Clear existing join entities. This will mark them for deletion.
        post.PostCategories.Clear();
        foreach (var categoryId in request.PostDto.CategoryIds)
        {
            if (await _unitOfWork.Categories.GetByIdAsync(categoryId) == null)
            {
                throw new NotFoundException($"Category with ID {categoryId} not found.");
            }
            // FIX: Only set CategoryId. Post will be inferred by EF Core.
            post.PostCategories.Add(new PostCategory { CategoryId = categoryId });
        }

        // Update Tags (remove old, add new)
        // Clear existing join entities. This will mark them for deletion.
        post.PostTags.Clear();
        foreach (var tagId in request.PostDto.TagIds)
        {
            if (await _unitOfWork.Tags.GetByIdAsync(tagId) == null)
            {
                throw new NotFoundException($"Tag with ID {tagId} not found.");
            }
            // FIX: Only set TagId. Post will be inferred by EF Core.
            post.PostTags.Add(new PostTag { TagId = tagId });
        }

        await _unitOfWork.Posts.UpdateAsync(post);
        await _unitOfWork.CommitAsync();

        // Re-fetch with details to ensure all relationships are loaded for the response DTO
        var updatedPost = await _unitOfWork.Posts.GetPostWithDetailsAsync(post.Id) ?? throw new InvalidOperationException("Failed to retrieve updated post details.");

        var postDto = _mapper.Map<PostDto>(updatedPost);
        postDto.LikeCount = updatedPost.Likes?.Count ?? 0;
        postDto.CommentCount = updatedPost.Comments?.Count ?? 0;

        return postDto;
    }
}