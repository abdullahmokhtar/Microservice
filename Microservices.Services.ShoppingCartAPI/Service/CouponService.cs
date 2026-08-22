using Microservices.Services.ShoppingCartAPI.Models.Dto;
using Microservices.Services.ShoppingCartAPI.Service.IService;

namespace Microservices.Services.ShoppingCartAPI.Service;

public class CouponService(IHttpClientFactory clientFactory) : ICouponService
{
    public async Task<CouponDto?> GetCoupon(string couponCode)
    {
        var client = clientFactory.CreateClient("Coupon");
        var response = await client.GetAsync($"/api/coupons/GetByCode/{couponCode}");
        var content = await response.Content.ReadAsStringAsync();
        var result = System.Text.Json.JsonSerializer.Deserialize<ResultDto<CouponDto>>(content, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return result?.Data;
    }
}
