using Microservices.Service.OrderAPI.Models.Dto;

namespace Microservices.Services.OrderAPI.Service.IService;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProducts();
}
