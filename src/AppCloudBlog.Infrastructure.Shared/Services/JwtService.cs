namespace AppCloudBlog.Infrastructure.Shared.Services
{
    public class JwtService : IJwtService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork; // To manage RefreshToken persistence

        public JwtService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResponseDto> GenerateJwtAndRefreshToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtSettings = _configuration.GetSection("JwtSettings");
            if (jwtSettings == null || 
                string.IsNullOrEmpty(jwtSettings) ||
                string.IsNullOrEmpty(jwtSettings["Issuer"]) || 
                string.IsNullOrEmpty(jwtSettings["Audience"]) ||
                string.IsNullOrEmpty(jwtSettings) || 
                string.IsNullOrEmpty(jwtSettings))
            {
                throw new InvalidOperationException("JWT settings are not configured correctly in appsettings.json.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenExpirationMinutes = Convert.ToDouble(jwtSettings);
            var tokenExpires = DateTime.UtcNow.AddMinutes(tokenExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: tokenExpires,
                signingCredentials: creds
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshTokenExpirationDays = Convert.ToDouble(jwtSettings);
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