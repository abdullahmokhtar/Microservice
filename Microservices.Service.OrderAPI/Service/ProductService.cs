using System.Text.Json;
using Microservices.Service.OrderAPI.Models.Dto;
using Microservices.Services.OrderAPI.Service.IService;

namespace Microservices.Services.OrderAPI.Service;

public class ProductService(IHttpClientFactory clientFactory) : IProductService
{
    public async Task<IEnumerable<ProductDto>> GetAllProducts()
    {
        var client = clientFactory.CreateClient("Product");
        var response = await client.GetAsync("/api/products");
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ResultDto<IEnumerable<ProductDto>>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return result?.Data ?? new List<ProductDto>();
    }
}
