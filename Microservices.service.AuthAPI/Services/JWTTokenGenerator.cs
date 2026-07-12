using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microservices.service.AuthAPI.Models;
using Microservices.service.AuthAPI.Services.Iservices;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Microservices.service.AuthAPI.Services
{
    public class JWTTokenGenerator(IOptions<JWTOptions> jWTOptions) : IJWTTokenGenerator
    {
        public string GenerateToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(jWTOptions.Value.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(jWTOptions.Value.ExpiryInDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jWTOptions.Value.Issuer,
                Audience = jWTOptions.Value.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
