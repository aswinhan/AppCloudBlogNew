namespace AppCloudBlog.Application.Features.Posts.Commands.CreatePost;

// 1. Command/Request Definition
public record CreatePostCommand(CreatePostDto PostDto, Guid AuthorId) : IRequest<PostDto>;

// 2. Validator for the Command
public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePostCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

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
           .MustAsync(BeUniqueSlug).WithMessage("The specified slug already exists.");

        RuleFor(x => x.PostDto.FeaturedImageUrl)
           .MaximumLength(2000).WithMessage("Featured image URL must not exceed 2000 characters.")
           .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).When(x => !string.IsNullOrEmpty(x.PostDto.FeaturedImageUrl)).WithMessage("Invalid Featured Image URL format.");

        RuleFor(x => x.PostDto.CategoryIds)
           .Must(ids => ids != null && ids.Count != 0).WithMessage("At least one category is required.");

        RuleFor(x => x.AuthorId)
           .NotEmpty().WithMessage("Author ID is required.");
    }

    private async Task<bool> BeUniqueSlug(string slug, CancellationToken cancellationToken)
    {
        var existingPost = await _unitOfWork.Posts.GetBySlugAsync(slug);
        return existingPost == null;
    }
}

// 3. Handler for the Command
public class CreatePostCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, HtmlSanitizer htmlSanitizer) : IRequestHandler<CreatePostCommand, PostDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly HtmlSanitizer _htmlSanitizer = htmlSanitizer; // Injected for content sanitization [1, 2]

    public async Task<PostDto> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        // Check if author exists and is active
        var author = await _unitOfWork.Users.GetUserWithDetailsAsync(request.AuthorId);
        if (author == null || !author.IsActive)
        {
            throw new NotFoundException("Author not found or inactive.");
        }

        // Sanitize HTML content before mapping to entity [1, 2]
        request.PostDto.Content = _htmlSanitizer.Sanitize(request.PostDto.Content);

        var post = _mapper.Map<Post>(request.PostDto);
        post.AuthorId = request.AuthorId;
        post.PublishDate = request.PostDto.IsPublished ? DateTime.UtcNow : default; // Set publish date if published immediately
        post.ViewCount = 0; // Initialize view count

        // Handle categories
        foreach (var categoryId in request.PostDto.CategoryIds)
        {
            if (await _unitOfWork.Categories.GetByIdAsync(categoryId) == null)
            {
                throw new NotFoundException($"Category with ID {categoryId} not found.");
            }
            post.PostCategories.Add(new PostCategory { CategoryId = categoryId, Post = post });
        }

        // Handle tags
        foreach (var tagId in request.PostDto.TagIds)
        {
            if (await _unitOfWork.Tags.GetByIdAsync(tagId) == null)
            {
                throw new NotFoundException($"Tag with ID {tagId} not found.");
            }
            post.PostTags.Add(new PostTag { TagId = tagId, Post = post });
        }

        await _unitOfWork.Posts.AddAsync(post);
        await _unitOfWork.CommitAsync();

        // Map the created post back to DTO for response
        var postDto = _mapper.Map<PostDto>(post);
        // Manually populate Author DTO as it's a nested object and might not be fully mapped by Mapster without explicit Include
        postDto.Author = _mapper.Map<UserDto>(author);
        postDto.LikeCount = 0; // New post, no likes yet
        postDto.IsLikedByUser = false;
        postDto.IsSavedByUser = false;
        postDto.Comments = []; // New post, no comments yet

        return postDto;
    }
}