using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OrderManagementSystem.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrderManagementSystem.Auth
{
    public class JwtTokenGenerator
    {
        private readonly IConfiguration _config;

        public JwtTokenGenerator(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");

            var secret = jwtSettings.GetValue<string>("SecretKey");
            var issuer = jwtSettings.GetValue<string>("Issuer");
            var audience = jwtSettings.GetValue<string>("Audience");
            var expiryMinutes = jwtSettings.GetValue<int?>("ExpiryMinutes") ?? 60;

            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("Configuration error: JwtSettings:SecretKey is not set.");
            if (string.IsNullOrWhiteSpace(issuer))
                throw new InvalidOperationException("Configuration error: JwtSettings:Issuer is not set.");
            if (string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("Configuration error: JwtSettings:Audience is not set.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username!),
                new Claim(ClaimTypes.Role, user.Role!),
                new Claim("UserId", user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
