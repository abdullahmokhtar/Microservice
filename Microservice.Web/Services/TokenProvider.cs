using Microservice.Web.Services.IServices;
using Microservice.Web.Utility;

namespace Microservice.Web.Services;

public class TokenProvider(IHttpContextAccessor contextAccessor) : ITokenProvider
{
    public void ClearToken()
    {
        contextAccessor.HttpContext?.Response.Cookies.Delete(SD.TokenCookie);
    }

    public string? GetToken()
    {
        string? token = null;
        contextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.TokenCookie, out token);
        return token;
    }

    public void SetToken(string token)
    {
        contextAccessor.HttpContext?.Response.Cookies.Append(SD.TokenCookie, token);
    }
}
