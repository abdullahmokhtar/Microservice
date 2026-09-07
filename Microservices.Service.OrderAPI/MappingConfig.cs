using AutoMapper;
using Microservices.Service.OrderAPI.Models;
using Microservices.Service.OrderAPI.Models.Dto;

namespace Microservices.Service.OrderAPI;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<OrderHeaderDto, CartHeaderDto>().ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal)).ReverseMap();
        CreateMap<CartDetailsDto, OrderDetailDto>()
            .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));
        CreateMap<OrderDetailDto, CartDetailsDto>();
        CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
        CreateMap<OrderDetail, OrderDetailDto>().ReverseMap();
    }
}
