using Microservice.Web.Models;
using Microservice.Web.Services.IServices;
using Microservice.Web.Utility;

namespace Microservice.Web.Services;

public class OrderService(IBaseService baseService, ServicesBaseURI servicesBaseURI) : IOrderService
{
    public async Task<ResultDto<OrderHeaderDto>> CreateOrderAsync(CartDto cart, CancellationToken cancellationToken = default)
    {
        return await baseService.SendAsync<OrderHeaderDto>(new RequestDto
        {
            APIType = ApiType.POST,
            Data = cart,
            URL = servicesBaseURI.OrderApi + "/api/orders"
        });
    }

    public async Task<ResultDto<StripeRequestDto>> CreateStripeSessionAsync(StripeRequestDto stripeRequest, CancellationToken cancellationToken = default)
    {
        return await baseService.SendAsync<StripeRequestDto>(new RequestDto
        {
            APIType = ApiType.POST,
            Data = stripeRequest,
            URL = servicesBaseURI.OrderApi + "/api/orders/CreateStripeSession"
        });
    }

    public async Task<ResultDto<OrderHeaderDto>> ValidateStipeSessionAsync(int orderHeaderId, CancellationToken cancellationToken = default)
    {
        return await baseService.SendAsync<OrderHeaderDto>(new RequestDto
        {
            APIType = ApiType.POST,
            Data = orderHeaderId,
            URL = servicesBaseURI.OrderApi + "/api/orders/ValidateStripeSession"
        });
    }
}
