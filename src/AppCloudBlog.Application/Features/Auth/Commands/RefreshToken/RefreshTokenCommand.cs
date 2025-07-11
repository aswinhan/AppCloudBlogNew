// Add a using alias for the RefreshToken entity to avoid ambiguity
using DomainRefreshToken = AppCloudBlog.Domain.Entities.RefreshToken;

namespace AppCloudBlog.Application.Features.Auth.Commands.RefreshToken;

// 1. Command/Request Definition
public record RefreshTokenCommand(RefreshTokenRequestDto RefreshTokenDto) : IRequest<AuthResponseDto>;

// 2. Validator for the Command
public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshTokenDto.RefreshToken)
        .NotEmpty().WithMessage("Refresh token is required.");
    }
}

// 3. Handler for the Command
public class RefreshTokenCommandHandler(
    IUnitOfWork unitOfWork,
    UserManager<ApplicationUser> userManager,
    IJwtService jwtService) : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtService _jwtService = jwtService;

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenString = request.RefreshTokenDto.RefreshToken;

        // Use the alias 'DomainRefreshToken' to refer to the entity type
        var storedRefreshToken = (await _unitOfWork.Repository<DomainRefreshToken>()
                                               .GetWhereAsync(rt => rt.Token == refreshTokenString))
                                               .FirstOrDefault();

        if (storedRefreshToken == null || !storedRefreshToken.IsActive)
        {
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        var user = await _userManager.FindByIdAsync(storedRefreshToken.UserId.ToString());
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedException("User associated with refresh token not found or inactive.");
        }

        // Revoke the old refresh token
        storedRefreshToken.Revoked = DateTime.UtcNow;
        // The GenericRepository.UpdateAsync will track this change.

        // Generate new JWT and refresh token
        var newAuthResponse = await _jwtService.GenerateJwtAndRefreshToken(user);

        // Link the old token to the new one for traceability (optional but good for security auditing)
        storedRefreshToken.ReplacedByToken = newAuthResponse.RefreshToken;
        // The GenericRepository.UpdateAsync will track this change.

        // Commit all changes in one transaction (revoking old token, adding new token)
        await _unitOfWork.CommitAsync();

        return newAuthResponse;
    }
}