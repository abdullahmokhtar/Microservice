using Microservice.Web.Models;

namespace Microservice.Web.Services.IServices;

public interface IOrderService
{
    public Task<ResultDto<OrderHeaderDto>> CreateOrderAsync(CartDto cart, CancellationToken cancellationToken = default);
    public Task<ResultDto<StripeRequestDto>> CreateStripeSessionAsync(StripeRequestDto stripeRequest, CancellationToken cancellationToken = default);
    public Task<ResultDto<OrderHeaderDto>> ValidateStipeSessionAsync(int orderHeaderId, CancellationToken cancellationToken = default);
}