using Microservice.Web.Models;

namespace Microservice.Web.Services.IServices;

public interface ICartService
{
    public Task<ResultDto<CartDto>> GetCartByUserIdAsync(string userId);
    public Task<ResultDto<CartDto>> UpsertCartAsync(CartDto cart);
    public Task<ResultDto<string>> RemoveFromCartAsync(int cartDetailsId);
    public Task<ResultDto<string>> ApplyCouponAsync(CartDto cart);
    public Task<ResultDto<bool>> EmailCartAsync(CartDto cart);
}

