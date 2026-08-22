using Microservices.Services.ShoppingCartAPI.Models.Dto;

namespace Microservices.Services.ShoppingCartAPI.Service.IService;

public interface ICouponService
{
    public Task<CouponDto?> GetCoupon(string couponCode);
}
