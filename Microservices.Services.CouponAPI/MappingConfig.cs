using AutoMapper;
using Microservices.Services.CouponAPI.Models;
using Microservices.Services.CouponAPI.Models.Dto;

namespace Microservices.Services.CouponAPI;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<Coupon, CouponDto>().ReverseMap();
    }
}
