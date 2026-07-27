using Microservice.Web.Models;

namespace Microservice.Web.Services.IServices
{
    public interface IBaseService
    {
        Task<ResultDto<T>> SendAsync<T>(RequestDto requestDto, bool withBearer = true);
    }
}
