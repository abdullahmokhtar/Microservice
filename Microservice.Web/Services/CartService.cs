using Microservice.Web.Models;
using Microservice.Web.Services.IServices;
using Microservice.Web.Utility;

namespace Microservice.Web.Services;

public class CartService(IBaseService baseService, ServicesBaseURI servicesBaseURI) : ICartService
{
    public async Task<ResultDto<string>> ApplyCouponAsync(CartDto cart)
    {
        return await baseService.SendAsync<string>(new RequestDto
        {
            APIType = ApiType.POST,
            Data = cart,
            URL = servicesBaseURI.ShoppingCartApi + "/api/cart/ApplyCoupon"
        });
    }

    public async Task<ResultDto<bool>> EmailCartAsync(CartDto cart)
    {
        return await baseService.SendAsync<bool>(new RequestDto
        {
            APIType = ApiType.POST,
            Data = cart,
            URL = servicesBaseURI.ShoppingCartApi + "/api/cart/EmailCartRequest"
        });
    }

    public async Task<ResultDto<CartDto>> GetCartByUserIdAsync(string userId)
    {
        return await baseService.SendAsync<CartDto>(new RequestDto
        {
            APIType = ApiType.GET,
            URL = $"{servicesBaseURI.ShoppingCartApi}/api/cart/GetCart/{userId}"
        });
    }

    public async Task<ResultDto<string>> RemoveFromCartAsync(int cartDetailsId)
    {
        return await baseService.SendAsync<string>(new RequestDto
        {
            APIType = ApiType.POST,
            URL = $"{servicesBaseURI.ShoppingCartApi}/api/cart/RemoveCart?cartDetailsId={cartDetailsId}"
        });
    }

    public Task<ResultDto<CartDto>> UpsertCartAsync(CartDto cart)
    {
        return baseService.SendAsync<CartDto>(new RequestDto
        {
            APIType = ApiType.POST,
            Data = cart,
            URL = servicesBaseURI.ShoppingCartApi + "/api/cart/UpsertCart"
        });
    }
}
