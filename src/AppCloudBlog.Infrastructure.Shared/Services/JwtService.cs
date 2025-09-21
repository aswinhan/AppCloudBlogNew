namespace AppCloudBlog.Infrastructure.Shared.Services
{
    public class JwtService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IUnitOfWork unitOfWork) : IJwtService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly IUnitOfWork _unitOfWork = unitOfWork; // To manage RefreshToken persistence

        public async Task<AuthResponseDto> GenerateJwtAndRefreshToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            };
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var jwtSettings = _configuration.GetSection("JwtSettings");
            if (jwtSettings == null ||
                string.IsNullOrEmpty(jwtSettings["Issuer"]) ||
                string.IsNullOrEmpty(jwtSettings["Audience"]) ||
                string.IsNullOrEmpty(jwtSettings["Secret"]) ||
                string.IsNullOrEmpty(jwtSettings["TokenExpirationMinutes"]) ||
                string.IsNullOrEmpty(jwtSettings["RefreshTokenExpirationDays"]))
            {
                throw new InvalidOperationException("JWT settings are not configured correctly in appsettings.json.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenExpirationMinutes = Convert.ToDouble(jwtSettings["TokenExpirationMinutes"]);
            var tokenExpires = DateTime.UtcNow.AddMinutes(tokenExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: tokenExpires,
                signingCredentials: creds
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshTokenExpirationDays = Convert.ToDouble(jwtSettings["RefreshTokenExpirationDays"]);
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(refreshTokenExpirationDays),
                Created = DateTime.UtcNow,
                UserId = user.Id
            };

            // Add the new refresh token to the database
            await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshToken);
            // Note: CommitAsync will be called by the command handler that uses this service
            // to ensure atomicity of the overall operation (e.g., login or registration).

            return new AuthResponseDto
            {
                UserId = user.Id.ToString(),
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                JwtToken = jwtToken,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpires = refreshToken.Expires,
                Roles = userRoles.ToList()
            };
        }
    }
}