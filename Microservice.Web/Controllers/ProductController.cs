using Microservice.Web.Models;
using Microservice.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Web.Controllers
{
    public class ProductController(IProductService productService) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            var response = await productService.GetAllProductsAsync();
            if (!response.Success)
            {
                TempData["error"] = response.Message;
            }
            return View(response.Data ?? new List<ProductDto>());
        }

        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDto model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var response = await productService.CreateProductAsync(model);
            if (response.Success)
            {
                TempData["success"] = response.Message;
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = response.Message;
            return View(model);
        }

        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var response = await productService.GetProductByIdAsync(productId);
            if (!response.Success)
            {
                TempData["error"] = response.Message;
                return NotFound();
            }
            return View(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(ProductDto model)
        {
            var response = await productService.DeleteProductAsync(model.ProductId);
            if (!response.Success)
            {
                TempData["error"] = response.Message;
                return View(model);
            }
            TempData["success"] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateProduct(int productId)
        {
            var response = await productService.GetProductByIdAsync(productId);
            if (!response.Success)
            {
                TempData["error"] = response.Message;
                return NotFound();
            }
            return View(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(ProductDto model)
        {
            var response = await productService.UpdateProductAsync(model);
            if (!response.Success)
            {
                TempData["error"] = response.Message;
                return View(model);
            }
            TempData["success"] = response.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
