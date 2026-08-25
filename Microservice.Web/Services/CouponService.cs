using Microservice.Web.Models;
using Microservice.Web.Services.IServices;
using Microservice.Web.Utility;

namespace Microservice.Web.Services;

public class CouponService(IBaseService baseService, ServicesBaseURI servicesBaseURI) : ICouponService
{
    public async Task<ResultDto<CouponDto>> CreateCouponAsync(CouponDto couponDto)
    {
        return await baseService.SendAsync<CouponDto>(new RequestDto
        {
            APIType = ApiType.POST,
            Data = couponDto,
            URL = servicesBaseURI.CouponAPI + "/api/coupons"
        });
    }

    public async Task<ResultDto<bool>> DeleteCouponAsync(int couponId)
    {
        return await baseService.SendAsync<bool>(new RequestDto
        {
            APIType = ApiType.DELETE,
            URL = $"{servicesBaseURI.CouponAPI}/api/coupons/{couponId}"
        });
    }

    public async Task<ResultDto<IEnumerable<CouponDto>>> GetAllCouponsAsync()
    {
        return await baseService.SendAsync<IEnumerable<CouponDto>>(new RequestDto
        {
            APIType = ApiType.GET,
            URL = servicesBaseURI.CouponAPI + "/api/coupons"
        });
    }

    public async Task<ResultDto<CouponDto>> GetCouponByCodeAsync(string couponCode)
    {
        return await baseService.SendAsync<CouponDto>(new RequestDto
        {
            APIType = ApiType.GET,
            URL = $"{servicesBaseURI.CouponAPI}/api/coupons/GetByCode/{couponCode}"
        });
    }

    public async Task<ResultDto<CouponDto>> GetCouponByIdAsync(int couponId)
    {
        return await baseService.SendAsync<CouponDto>(new RequestDto
        {
            APIType = ApiType.GET,
            URL = $"{servicesBaseURI.CouponAPI}/api/coupons/{couponId}"
        });
    }

    public async Task<ResultDto<CouponDto>> UpdateCouponAsync(CouponDto couponDto)
    {
        return await baseService.SendAsync<CouponDto>(new RequestDto
        {
            APIType = ApiType.PUT,
            Data = couponDto,
            URL = servicesBaseURI.CouponAPI + "/api/coupons"
        });
    }
}
