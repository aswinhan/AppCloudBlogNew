namespace AppCloudBlog.Application.Features.Tags.Commands.UpdateTag;

// 1. Command/Request Definition
public record UpdateTagCommand(Guid Id, UpdateTagDto TagDto) : IRequest<TagDto>;

// 2. Validator for the Command
public class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTagCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
         .NotEmpty().WithMessage("Tag ID is required.");

        RuleFor(x => x.TagDto.Name)
         .NotEmpty().WithMessage("Tag name is required.")
         .MaximumLength(50).WithMessage("Tag name must not exceed 50 characters.")
         .MustAsync(async (command, name, cancellationToken) => await BeUniqueTagName(command.Id, name, cancellationToken)).WithMessage("The specified tag name already exists.");

        RuleFor(x => x.TagDto.Slug)
         .NotEmpty().WithMessage("Tag slug is required.")
         .MaximumLength(50).WithMessage("Tag slug must not exceed 50 characters.")
         .MustAsync(async (command, slug, cancellationToken) => await BeUniqueTagSlug(command.Id, slug, cancellationToken)).WithMessage("The specified tag slug already exists.");
    }

    private async Task<bool> BeUniqueTagName(Guid tagId, string name, CancellationToken _)
    {
        var existingTag = await _unitOfWork.Tags.GetWhereAsync(t => t.Name == name);
        return existingTag == null || !existingTag.Any() || existingTag[0].Id == tagId;
    }

    private async Task<bool> BeUniqueTagSlug(Guid tagId, string slug, CancellationToken _)
    {
        var existingTag = await _unitOfWork.Tags.GetBySlugAsync(slug);
        return existingTag == null || existingTag.Id == tagId;
    }
}

// 3. Handler for the Command
public class UpdateTagCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateTagCommand, TagDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<TagDto> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(request.Id) ?? throw new NotFoundException($"Tag with ID {request.Id} not found.");
        _mapper.Map(request.TagDto, tag); // Map DTO properties to existing tag entity

        await _unitOfWork.Tags.UpdateAsync(tag);
        await _unitOfWork.CommitAsync();

        var tagDto = _mapper.Map<TagDto>(tag);
        // Re-fetch post count if needed, or update manually if performance critical
        tagDto.PostCount = await _unitOfWork.Posts.GetPostsByTagIdAsync(tag.Id).ContinueWith(t => t.Result.Count, cancellationToken);

        return tagDto;
    }
}