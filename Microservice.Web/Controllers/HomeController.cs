using System.Diagnostics;
using Microservice.Web.Models;
using Microservice.Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Web.Controllers
{
    public class HomeController(IProductService productService, ICartService cartService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var response = await productService.GetAllProductsAsync();
            if (!response.Success)
            {
                TempData["error"] = response.Message;
            }
            return View(response.Data ?? new List<ProductDto>());
        }

        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            var response = await productService.GetProductByIdAsync(productId);
            if (!response.Success)
            {
                TempData["error"] = response.Message;
                return NotFound();
            }
            return View(response.Data);
        }

        [Authorize]
        [HttpPost("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDto productDto)
        {
            var cart = new CartDto
            {
                CartHeader = new CartHeaderDto
                {
                    UserId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                }
            };
            var cartDetails = new CartDetailsDto
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId
            };
            cart.CartDetails = new List<CartDetailsDto> { cartDetails };
            var response = await cartService.UpsertCartAsync(cart);
            if (!response.Success)
            {
                TempData["error"] = response.Message;
                return RedirectToAction(nameof(Details), new { productId = productDto.ProductId });
            }
            TempData["success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
