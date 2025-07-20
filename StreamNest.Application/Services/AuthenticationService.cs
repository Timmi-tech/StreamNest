using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StreamNest.Application.DTOs;
using StreamNest.Application.Services.Contracts;
using StreamNest.Domain.Contracts;
using StreamNest.Domain.Entities.ConfigurationsModels;
using StreamNest.Domain.Entities.Models;
using System.IdentityModel.Tokens.Jwt; 
using Microsoft.IdentityModel.Tokens; 


namespace StreamNest.Application.Services
{
    internal sealed class AuthenticationService : IAuthenticationService
    {
        private readonly ILoggerManager? _logger;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<JwtConfiguration> _configuration;
        private readonly JwtConfiguration _jwtConfiguration;
        private User? _user;

        public AuthenticationService(ILoggerManager logger, UserManager<User> userManager, IOptions<JwtConfiguration> configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _configuration = configuration;
            _jwtConfiguration = _configuration.Value;
        }
        public async Task<IdentityResult> RegisterUser(UserForRegistrationDto
        userForRegistration)
        {
            var user = new User
            {
                FirstName = userForRegistration.Firstname,
                LastName = userForRegistration.Lastname,
                UserName = userForRegistration.Username,
                Email = userForRegistration.Email,
                Role = UserRole.Consumer
            };
            var result = await _userManager.CreateAsync(user, userForRegistration.Password);
            return result;
        }
        public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuth)
        {
            _user = await _userManager.FindByEmailAsync(userForAuth.Email);
            if (_user == null)
            {
                _logger?.LogWarn($"Authentication failed. No user found with email: {userForAuth.Email}");
                return false;
            }
            if (!await _userManager.CheckPasswordAsync(_user, userForAuth.Password))
            {
                _logger?.LogWarn($"Authentication failed. Invalid password for user: {userForAuth.Email}");
                return false;
            }
            return true;
        }
        public async Task<TokenDto> CreateToken(bool populateExp)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);


            var refreshToken = GenerateRefreshToken();

            _user.RefreshToken = HashRefreshToken(refreshToken);

            if (populateExp)
            {
                _user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            }

            await _userManager.UpdateAsync(_user);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return new TokenDto(accessToken, refreshToken);
        }
        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_jwtConfiguration.Secret);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _user.Id),
                new Claim(ClaimTypes.Name, _user.UserName),
                new Claim(ClaimTypes.Role, _user.Role.ToString()) // Add this line
            };
            return claims;
        }
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims) 
        { 
            var tokenOptions = new JwtSecurityToken 
            ( 
                issuer: _jwtConfiguration.ValidIssuer,
                audience: _jwtConfiguration.ValidAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
                signingCredentials: signingCredentials
            ); 
                return tokenOptions;
            }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);  
            }
        }
        private string HashRefreshToken(string refreshToken)
        {
            using (var sha256 = SHA256.Create())
            {
                var tokenBytes = Encoding.UTF8.GetBytes(refreshToken);
                var hashBytes = sha256.ComputeHash(tokenBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
        // Add the GetPrincipalFromExpiredToken method to the AuthenticationService class.
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Secret)),
            ValidateLifetime = false,
            ValidIssuer = _jwtConfiguration.ValidIssuer,
            ValidAudience = _jwtConfiguration.ValidAudience,
            ClockSkew = TimeSpan.FromMinutes(5) 
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || 
        !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token Format");
        }

        return principal;   
        }
        
        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
            var username = principal.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                throw new SecurityTokenException("Invalid token - username not found");

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                throw new SecurityTokenException("Invalid token - user not found");

            var refreshTokenHash = HashRefreshToken(tokenDto.RefreshToken);
            if (user.RefreshToken != refreshTokenHash)
            {
                _logger?.LogWarn($"Invalid refresh token attempt for user: {username}");
                throw new SecurityTokenException("Invalid refresh token");
            }
            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                _logger?.LogWarn($"Expired refresh token attempt for user: {username}");
                throw new SecurityTokenException("Refresh token expired");
            }

            _user = user;
            return await CreateToken(populateExp: true);
        }
    }
    
}