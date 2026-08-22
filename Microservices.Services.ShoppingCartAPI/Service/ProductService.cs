using Microservices.Services.ShoppingCartAPI.Models.Dto;
using Microservices.Services.ShoppingCartAPI.Service.IService;

namespace Microservices.Services.ShoppingCartAPI.Service;

public class ProductService(IHttpClientFactory clientFactory) : IProductService
{
    public async Task<IEnumerable<ProductDto>> GetAllProducts()
    {
        var client = clientFactory.CreateClient("Product");
        var response = await client.GetAsync("/api/products");
        var content = await response.Content.ReadAsStringAsync();
        var result = System.Text.Json.JsonSerializer.Deserialize<ResultDto<IEnumerable<ProductDto>>>(content, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return result?.Data ?? new List<ProductDto>();
    }
}
