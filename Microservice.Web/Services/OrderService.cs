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

    public async Task<ResultDto<OrderHeaderDto>> Get(int id, CancellationToken cancellationToken = default)
    {
        return await baseService.SendAsync<OrderHeaderDto>(new RequestDto
        {
            APIType = ApiType.GET,
            URL = servicesBaseURI.OrderApi + "/api/orders/" + id
        });
    }

    public async Task<ResultDto<IEnumerable<OrderHeaderDto>>> GetAll(string userId, OrderStatus? status, CancellationToken cancellationToken = default)
    {
        return await baseService.SendAsync<IEnumerable<OrderHeaderDto>>(new RequestDto
        {
            APIType = ApiType.GET,
            URL = $"{servicesBaseURI.OrderApi}/api/orders?userId={userId}&status={status}"
        });
    }

    public Task<ResultDto<OrderHeaderDto>> UpdateStatus(int orderHeaderId, OrderStatus status, CancellationToken cancellationToken = default)
    {
        return baseService.SendAsync<OrderHeaderDto>(new RequestDto
        {
            APIType = ApiType.POST,
            Data = status,
            URL = servicesBaseURI.OrderApi + "/api/orders/UpdateStatus/" + orderHeaderId
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
