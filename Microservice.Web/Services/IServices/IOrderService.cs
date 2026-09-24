using Microservice.Web.Models;

namespace Microservice.Web.Services.IServices;

public interface IOrderService
{
    public Task<ResultDto<OrderHeaderDto>> CreateOrderAsync(CartDto cart, CancellationToken cancellationToken = default);
    public Task<ResultDto<StripeRequestDto>> CreateStripeSessionAsync(StripeRequestDto stripeRequest, CancellationToken cancellationToken = default);
    public Task<ResultDto<OrderHeaderDto>> ValidateStipeSessionAsync(int orderHeaderId, CancellationToken cancellationToken = default);
    public Task<ResultDto<IEnumerable<OrderHeaderDto>>> GetAll(string userId, OrderStatus? status, CancellationToken cancellationToken = default);
    public Task<ResultDto<OrderHeaderDto>> Get(int id, CancellationToken cancellationToken = default);
    public Task<ResultDto<OrderHeaderDto>> UpdateStatus(int orderHeaderId, OrderStatus status, CancellationToken cancellationToken = default);
}