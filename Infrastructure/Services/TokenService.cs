using Application.Repositories.Base;
using Domain.Entities;
using Domain.Models.Common;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
namespace Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IOptions<JWT> _jwtSettings;
        public TokenService(IOptions<JWT> jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public Task<string> GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.Key ?? string.Empty));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _jwtSettings.Value.Issuer,
                _jwtSettings.Value.Audience,
                claims,
                expires: DateTime.Now.AddHours(_jwtSettings.Value.ExpiresIn),
                signingCredentials: credentials
            );
            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public Task<string> GenerateRefreshToken(User user)
        {
            var refreshToken = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(refreshToken);
            }
            return Task.FromResult(Convert.ToBase64String(refreshToken));
        }
    }
}