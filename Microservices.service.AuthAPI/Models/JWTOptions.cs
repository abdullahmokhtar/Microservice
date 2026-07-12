namespace Microservices.service.AuthAPI.Models
{
    public class JWTOptions
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryInDays { get; set; }
    }
}
