using AutoMapper;
using Microservice.MessageBus;
using Microservices.Service.OrderAPI.Data;
using Microservices.Service.OrderAPI.Models;
using Microservices.Service.OrderAPI.Models.Dto;
using Microservices.Service.OrderAPI.Utlity;
using Microservices.Services.OrderAPI.Utlity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Microservices.Service.OrderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController(IMapper mapper, AppDbContext context, IOptions<StripeApiKey> stripeApiKey, IMessageBus messageBus, IOptions<TopicAndQueueNames> topicAndQueueNames, IOptions<AzureConfig> azureConfig) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetOrders(string userId, OrderStatus? status, CancellationToken cancellationToken = default)
    {
        var orderHeadersQuery = context.OrderHeaders.AsNoTracking();

        if (!User.IsInRole(SD.RoleAdmin))
        {
            orderHeadersQuery = orderHeadersQuery.Where(o => o.UserId == userId);
        }
        if (status is not null)
        {
            orderHeadersQuery = orderHeadersQuery.Where(o => o.Status == status);
        }
        var orderHeaders = await orderHeadersQuery.Include(o => o.OrderDetails)
          .OrderByDescending(o => o.OrderHeaderId)
            .ToListAsync(cancellationToken);
        var orderHeaderDtos = mapper.Map<IEnumerable<OrderHeaderDto>>(orderHeaders);
        return Ok(ResultDto<IEnumerable<OrderHeaderDto>>.SuccessResult(orderHeaderDtos));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrder(int id, CancellationToken cancellationToken = default)
    {
        var orderHeader = await context.OrderHeaders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.OrderHeaderId == id, cancellationToken);
        if (orderHeader is null)
        {
            return NotFound(ResultDto<OrderHeaderDto>.FailureResult("Order not found"));
        }
        var orderHeaderDto = mapper.Map<OrderHeaderDto>(orderHeader);
        return Ok(ResultDto<OrderHeaderDto>.SuccessResult(orderHeaderDto));
    }
    [HttpPost]
    public async Task<IActionResult> CreateOrder(CartDto cart, CancellationToken cancellationToken = default)
    {
        var orderHeaderDto = mapper.Map<OrderHeaderDto>(cart.CartHeader);
        orderHeaderDto.OrderDetails = mapper.Map<IEnumerable<OrderDetailDto>>(cart.CartDetails);

        var orderHeader = mapper.Map<OrderHeader>(orderHeaderDto);
        context.OrderHeaders.Add(orderHeader);
        await context.SaveChangesAsync(cancellationToken);
        orderHeaderDto.OrderHeaderId = orderHeader.OrderHeaderId;
        return Ok(ResultDto<OrderHeaderDto>.SuccessResult(orderHeaderDto));
    }

    [HttpPost("CreateStripeSession")]
    public async Task<IActionResult> CreateStripeSession(StripeRequestDto stripeRequest, CancellationToken cancellationToken = default)
    {
        var options = new SessionCreateOptions
        {
            SuccessUrl = stripeRequest.ApprovedURL,
            CancelUrl = stripeRequest.CancelURL,
            LineItems = stripeRequest.OrderHeader.OrderDetails.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(item.Price * 100),
                    Currency = "egp",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Product.Name,
                        Images = [item.Product.ImageUrl],
                    },
                },
                Quantity = item.Count,
            }).ToList(),
            Discounts = stripeRequest.OrderHeader.CouponCode is not null ? new List<SessionDiscountOptions>
            {
                new SessionDiscountOptions
                {
                    Coupon = stripeRequest.OrderHeader.CouponCode
                }
            } : null,
            Mode = "payment"
        };
        var client = new StripeClient(stripeApiKey.Value.ApiKey);
        var service = client.V1.Checkout.Sessions;
        Session session = service.Create(options);
        stripeRequest.SessionURL = session.Url;
        var orderHeader = await context.OrderHeaders.FirstOrDefaultAsync(o => o.OrderHeaderId == stripeRequest.OrderHeader.OrderHeaderId, cancellationToken);
        stripeRequest.SessionId = session.Id;
        orderHeader.StripeSessionId = session.Id;
        await context.SaveChangesAsync(cancellationToken);
        return Ok(ResultDto<StripeRequestDto>.SuccessResult(stripeRequest));
    }

    [HttpPost("ValidateStripeSession")]
    public async Task<IActionResult> ValidateStripeSession([FromBody] int orderHeaderId, CancellationToken cancellationToken = default)
    {
        var orderheader = await context.OrderHeaders.FirstOrDefaultAsync(o => o.OrderHeaderId == orderHeaderId, cancellationToken);
        var client = new StripeClient(stripeApiKey.Value.ApiKey);
        var service = client.V1.Checkout.Sessions;
        Session session = service.Get(orderheader.StripeSessionId);
        var paymentIntentService = client.V1.PaymentIntents;
        var paymentIntent = paymentIntentService.Get(session.PaymentIntentId);
        if (paymentIntent.Status == "succeeded")
        {
            orderheader.PaymentIntntId = paymentIntent.Id;
            orderheader.Status = OrderStatus.Approved;
            await context.SaveChangesAsync(cancellationToken);
            await CreateRewardForOrder(orderheader);
            return Ok(ResultDto<OrderHeaderDto>.SuccessResult(mapper.Map<OrderHeaderDto>(orderheader)));
        }
        return BadRequest(ResultDto<StripeRequestDto>.FailureResult("Payment not completed"));
    }

    private async Task CreateRewardForOrder(OrderHeader orderHeader)
    {
        var rewardDto = new RewardDto
        {
            UserId = orderHeader.UserId,
            RewardActivity = Convert.ToInt16(orderHeader.OrderTotal),
            OrderId = orderHeader.OrderHeaderId
        };
        await messageBus.PublishMessage(azureConfig.Value.ConnectionString, rewardDto, topicAndQueueNames.Value.OrderCreatedTopic);
    }

    [HttpPost("UpdateStatus/{id:int}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatus status, CancellationToken cancellationToken = default)
    {
        var orderHeader = await context.OrderHeaders.FirstOrDefaultAsync(o => o.OrderHeaderId == id, cancellationToken);
        if (orderHeader is null)
        {
            return NotFound(ResultDto<OrderHeaderDto>.FailureResult("Order not found"));
        }
        if (status == OrderStatus.Cancelled)
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = orderHeader.PaymentIntntId,
                Reason = RefundReasons.RequestedByCustomer
            };
            var client = new StripeClient(stripeApiKey.Value.ApiKey);
            var refundService = client.V1.Refunds;
            var refund = refundService.Create(options);
            orderHeader.Status = OrderStatus.Refunded;
        }
        else
        {
            orderHeader.Status = status;
        }
        await context.SaveChangesAsync(cancellationToken);
        return Ok(ResultDto<OrderHeaderDto>.SuccessResult(mapper.Map<OrderHeaderDto>(orderHeader)));
    }
}
