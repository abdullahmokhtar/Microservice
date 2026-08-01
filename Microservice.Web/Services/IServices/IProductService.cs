using Microservice.Web.Models;

namespace Microservice.Web.Services.IServices;

public interface IProductService
{
    public Task<ResultDto<IEnumerable<ProductDto>>> GetAllProductsAsync();
    public Task<ResultDto<ProductDto>> GetProductByIdAsync(int productId);
    public Task<ResultDto<ProductDto>> CreateProductAsync(ProductDto productDto);
    public Task<ResultDto<ProductDto>> UpdateProductAsync(ProductDto productDto);
    public Task<ResultDto<bool>> DeleteProductAsync(int productId);
}

