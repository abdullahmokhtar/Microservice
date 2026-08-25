namespace Microservice.Web.Utility
{
    public class SD
    {
        public static string RoleAdmin { get; set; } = "ADMIN";
        public static string RoleCustomer { get; set; } = "CUSTOMER";
        public static string TokenCookie { get; set; } = "JWTToken";

    }
    public enum ApiType
    {
        GET,
        POST,
        PUT,
        DELETE
    }
}
