using AutoMapper;
using Microservice.MessageBus;
using Microservices.Services.ShoppingCartAPI.Data;
using Microservices.Services.ShoppingCartAPI.Models;
using Microservices.Services.ShoppingCartAPI.Models.Dto;
using Microservices.Services.ShoppingCartAPI.Service.IService;
using Microservices.Services.ShoppingCartAPI.Utlity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Microservices.Services.ShoppingCartAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController(AppDbContext context, IProductService productService, ICouponService couponService, IMessageBus messageBus, IOptions<TopicAndQueueNames> options, IConfiguration configuration, IMapper mapper) : ControllerBase
{
    [HttpGet("GetCart/{userId}")]
    public async Task<IActionResult> GetCart(string userId)
    {
        var cart = await context.CartHeaders.Include(e => e.CartDetails).FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart is null)
            return NotFound(ResultDto<string>.FailureResult("Cart not found"));
        var cartHeaderDto = mapper.Map<CartHeaderDto>(cart);
        var cartDetailsDto = mapper.Map<List<CartDetailsDto>>(cart.CartDetails);
        CartDto result = new CartDto
        {
            CartHeader = cartHeaderDto,
            CartDetails = cartDetailsDto
        };
        var products = await productService.GetAllProducts();
        result.CartDetails = result.CartDetails.Select(cd =>
        {
            var product = products.FirstOrDefault(p => p.ProductId == cd.ProductId);
            if (product is not null)
            {
                cd.Product = product;
            }
            return cd;
        }).ToList();
        result.CartHeader.CartTotal = result.CartDetails.Sum(d =>
        {
            var product = products.FirstOrDefault(p => p.ProductId == d.ProductId);
            return product.Price * d.Count;
        });
        result = await ApplyCouponDiscount(result);
        return Ok(ResultDto<CartDto>.SuccessResult(result));
    }

    private async Task<CartDto> ApplyCouponDiscount(CartDto cart)
    {
        if (string.IsNullOrEmpty(cart.CartHeader.CouponCode))
            return cart;
        var coupon = await couponService.GetCoupon(cart.CartHeader.CouponCode);
        if (coupon is null || coupon.MinAmount > cart.CartHeader.CartTotal)
            return cart;
        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
        cart.CartHeader.Discount = coupon.DiscountAmount;
        return cart;
    }

    [HttpPost("ApplyCoupon")]
    public async Task<IActionResult> ApplyCoupon([FromBody] CartDto cartDto)
    {
        var cart = await context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == cartDto.CartHeader.UserId);
        if (cart is null)
            return NotFound(ResultDto<string>.FailureResult("Cart not found"));
        cart.CouponCode = cartDto.CartHeader.CouponCode;
        context.CartHeaders.Update(cart);
        await context.SaveChangesAsync();
        return Ok(ResultDto<string>.SuccessResult("Coupon applied successfully"));
    }

    [HttpPost("EmailCartRequest")]
    public async Task<IActionResult> EmailCartRequest([FromBody] CartDto cartDto)
    {
        await messageBus.PublishMessage(configuration["Azure:ConnectionString"], cartDto, options.Value.EmailShoppingCart);
        return Ok(ResultDto<bool>.SuccessResult(true));
    }

    [HttpPost("RemoveCoupon")]
    public async Task<IActionResult> RemoveCoupon([FromBody] CartDto cartDto)
    {
        var cart = await context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == cartDto.CartHeader.UserId);
        if (cart is null)
            return NotFound(ResultDto<string>.FailureResult("Cart not found"));
        cart.CouponCode = string.Empty;
        context.CartHeaders.Update(cart);
        await context.SaveChangesAsync();
        return Ok(ResultDto<string>.SuccessResult("Coupon removed successfully"));
    }

    [HttpPost("UpsertCart")]
    public async Task<IActionResult> Upsert(CartDto cartDto)
    {
        var cart = await context.CartHeaders.Include(e => e.CartDetails).FirstOrDefaultAsync(c => c.UserId == cartDto.CartHeader.UserId);
        if (cart is null)
        {
            var cartHeader = mapper.Map<CartHeader>(cartDto.CartHeader);
            var cartDetails = mapper.Map<List<CartDetails>>(cartDto.CartDetails);
            cartHeader.CartDetails = cartDetails;
            context.CartHeaders.Add(cartHeader);
            await context.SaveChangesAsync();
            cart = cartHeader;
        }
        else
        {
            foreach (var detail in cartDto.CartDetails)
            {
                var existingDetail = cart.CartDetails.FirstOrDefault(d => d.ProductId == detail.ProductId);
                if (existingDetail is null)
                {
                    if (detail.Count < 1)
                        continue;
                    // Add new detail
                    detail.CartHeaderId = cart.CartHeaderId;
                    context.CartDetails.Add(mapper.Map<CartDetails>(detail));
                }
                else
                {
                    // Update existing detail
                    existingDetail.Count += detail.Count;
                    if (existingDetail.Count < 1)
                        cart.CartDetails.Remove(existingDetail);
                    else
                        context.CartDetails.Update(existingDetail);
                }
            }
            await context.SaveChangesAsync();
        }
        var cartHeaderDto = mapper.Map<CartHeaderDto>(cart);
        var cartDetailsDto = mapper.Map<List<CartDetailsDto>>(cart.CartDetails);
        CartDto result = new CartDto
        {
            CartHeader = cartHeaderDto,
            CartDetails = cartDetailsDto
        };
        return Ok(ResultDto<CartDto>.SuccessResult(result));
    }

    [HttpPost("RemoveCart")]
    public IActionResult RemoveCart(int cartDetailsId)
    {
        var cartDetail = context.CartDetails.FirstOrDefault(c => c.CartDetailsId == cartDetailsId);
        if (cartDetail is null)
            return NotFound(ResultDto<string>.FailureResult("Cart detail not found"));
        var cartHeader = context.CartHeaders.Include(e => e.CartDetails).FirstOrDefault(c => c.CartHeaderId == cartDetail.CartHeaderId);

        if (cartHeader.CartDetails.Count == 1)
        {
            context.CartHeaders.Remove(cartHeader);
        }
        else
        {
            context.CartDetails.Remove(cartDetail);
        }
        context.SaveChangesAsync();
        return Ok(ResultDto<string>.SuccessResult("Cart detail removed successfully"));
    }
}
