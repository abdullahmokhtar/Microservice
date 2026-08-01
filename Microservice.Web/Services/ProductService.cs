using Microservice.Web.Models;
using Microservice.Web.Services.IServices;
using Microservice.Web.Utility;

namespace Microservice.Web.Services
{
    public class ProductService(IBaseService baseService) : IProductService
    {
        public async Task<ResultDto<ProductDto>> CreateProductAsync(ProductDto productDto)
        {
            return await baseService.SendAsync<ProductDto>(new RequestDto
            {
                APIType = ApiType.POST,
                Data = productDto,
                URL = SD.ProductAPIBase + "/api/products"
            });
        }

        public async Task<ResultDto<bool>> DeleteProductAsync(int productId)
        {
            return await baseService.SendAsync<bool>(new RequestDto
            {
                APIType = ApiType.DELETE,
                URL = $"{SD.ProductAPIBase}/api/products/{productId}"
            });
        }

        public async Task<ResultDto<IEnumerable<ProductDto>>> GetAllProductsAsync()
        {
            return await baseService.SendAsync<IEnumerable<ProductDto>>(new RequestDto
            {
                APIType = ApiType.GET,
                URL = SD.ProductAPIBase + "/api/products"
            });
        }

        public async Task<ResultDto<ProductDto>> GetProductByIdAsync(int productId)
        {
            return await baseService.SendAsync<ProductDto>(new RequestDto
            {
                APIType = ApiType.GET,
                URL = $"{SD.ProductAPIBase}/api/products/{productId}"
            });
        }

        public async Task<ResultDto<ProductDto>> UpdateProductAsync(ProductDto productDto)
        {
            return await baseService.SendAsync<ProductDto>(new RequestDto
            {
                APIType = ApiType.PUT,
                Data = productDto,
                URL = SD.ProductAPIBase + "/api/products"
            });
        }
    }
}
