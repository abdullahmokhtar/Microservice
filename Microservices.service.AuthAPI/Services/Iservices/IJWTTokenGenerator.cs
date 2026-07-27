using Microservices.service.AuthAPI.Models;

namespace Microservices.service.AuthAPI.Services.Iservices
{
    public interface IJWTTokenGenerator
    {
        public string GenerateToken(ApplicationUser user, IEnumerable<string> roles);
    }
}
