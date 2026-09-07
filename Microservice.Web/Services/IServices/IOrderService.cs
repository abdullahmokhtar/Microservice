using Microservice.Web.Models;

namespace Microservice.Web.Services.IServices;

public interface IOrderService
{
    public Task<ResultDto<OrderHeaderDto>> CreateOrderAsync(CartDto cart, CancellationToken cancellationToken = default);
}

