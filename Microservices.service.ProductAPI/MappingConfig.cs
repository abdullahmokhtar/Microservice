using AutoMapper;
using Microservices.Services.ProductAPI.Models;
using Microservices.Services.ProductAPI.Models.Dto;

namespace Microservices.Services.ProductAPI;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
    }
}
