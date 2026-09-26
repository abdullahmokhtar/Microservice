using Microservice.Web.Utility;

namespace Microservice.Web.Models;

public class RequestDto
{
    public ApiType APIType { get; set; } = ApiType.GET;
    public string URL { get; set; } = null!;
    public object Data { get; set; }
    public string AccessToken { get; set; }
    public ContentType ContentType { get; set; } = ContentType.Json;
}
