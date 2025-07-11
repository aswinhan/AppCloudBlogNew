namespace AppCloudBlog.Application.Features.Tags.Commands.CreateTag;

// 1. Command/Request Definition
public record CreateTagCommand(CreateTagDto TagDto) : IRequest<TagDto>;

// 2. Validator for the Command
public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTagCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.TagDto.Name)
         .NotEmpty().WithMessage("Tag name is required.")
         .MaximumLength(50).WithMessage("Tag name must not exceed 50 characters.")
         .MustAsync(BeUniqueTagName).WithMessage("The specified tag name already exists.");

        RuleFor(x => x.TagDto.Slug)
         .NotEmpty().WithMessage("Tag slug is required.")
         .MaximumLength(50).WithMessage("Tag slug must not exceed 50 characters.")
         .MustAsync(BeUniqueTagSlug).WithMessage("The specified tag slug already exists.");
    }

    private async Task<bool> BeUniqueTagName(string name, CancellationToken _)
    {
        var existingTag = await _unitOfWork.Tags.GetWhereAsync(t => t.Name == name);
        return existingTag == null || !existingTag.Any();
    }

    private async Task<bool> BeUniqueTagSlug(string slug, CancellationToken _)
    {
        var existingTag = await _unitOfWork.Tags.GetBySlugAsync(slug);
        return existingTag == null;
    }
}

// 3. Handler for the Command
public class CreateTagCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateTagCommand, TagDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<TagDto> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = _mapper.Map<Tag>(request.TagDto);

        await _unitOfWork.Tags.AddAsync(tag);
        await _unitOfWork.CommitAsync();

        var tagDto = _mapper.Map<TagDto>(tag);
        tagDto.PostCount = 0; // New tag, no posts yet

        return tagDto;
    }
}