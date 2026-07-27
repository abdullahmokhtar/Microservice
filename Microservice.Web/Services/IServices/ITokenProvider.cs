namespace Microservice.Web.Services.IServices;

public interface ITokenProvider
{
    public void SetToken(string token);
    string? GetToken();
    void ClearToken();
}
