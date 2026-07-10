using Microservice.Web.Models;

namespace Microservice.Web.Services.IServices;

public interface ICouponService
{
    public Task<ResultDto<IEnumerable<CouponDto>>> GetAllCouponsAsync();
    public Task<ResultDto<CouponDto>> GetCouponByCodeAsync(string couponCode);
    public Task<ResultDto<CouponDto>> GetCouponByIdAsync(int couponId);
    public Task<ResultDto<CouponDto>> CreateCouponAsync(CouponDto couponDto);
    public Task<ResultDto<CouponDto>> UpdateCouponAsync(CouponDto couponDto);
    public Task<ResultDto<bool>> DeleteCouponAsync(int couponId);
}

