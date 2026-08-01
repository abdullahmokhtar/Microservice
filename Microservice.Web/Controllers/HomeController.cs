using System.Diagnostics;
using Microservice.Web.Models;
using Microservice.Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Web.Controllers
{
    public class HomeController(IProductService productService) : Controller
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
