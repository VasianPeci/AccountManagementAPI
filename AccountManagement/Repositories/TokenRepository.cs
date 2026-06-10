using Microsoft.AspNetCore.Identity;
using AccountManagement.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccountManagement.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration configuration;
        private readonly AccountManagementDbContext dbContext;

        public TokenRepository(IConfiguration configuration, AccountManagementDbContext dbContext)
        {
            this.configuration = configuration;
            this.dbContext = dbContext;
        }

        public string CreateJWTToken(IdentityUser user, List<string> roles)
        {
            var clientId = dbContext.Clients
                .FirstOrDefault(client => client.UserId == user.Id)
                ?.Id
                .ToString();

            // CLAIMS
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            if (!string.IsNullOrEmpty(clientId))
            {
                claims.Add(new Claim("clientId", clientId));
            }

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // SIGNING KEY
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"])
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            // TOKEN CREATION
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
