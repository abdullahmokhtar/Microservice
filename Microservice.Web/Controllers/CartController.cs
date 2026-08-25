using System.Security.Claims;
using Microservice.Web.Models;
using Microservice.Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Web.Controllers;

public class CartController(ICartService cartService) : Controller
{
    [Authorize]
    public async Task<IActionResult> Index()
    {
        return View(await GetCart());
    }

    private async Task<CartDto?> GetCart()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        var result = await cartService.GetCartByUserIdAsync(userId);
        return result?.Data;
    }

    public async Task<IActionResult> RemoveItem(int id)
    {
        var result = await cartService.RemoveFromCartAsync(id);
        if (result.Success)
        {
            TempData["success"] = result.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
    {
        cartDto.CartDetails = new List<CartDetailsDto>();
        var result = await cartService.ApplyCouponAsync(cartDto);
        if (result.Success)
        {
            TempData["success"] = result.Data;
        }
        else
        {
            TempData["error"] = result.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> EmailCart(CartDto cartDto)
    {
        var cart = await GetCart();
        cart?.CartHeader.Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var result = await cartService.EmailCartAsync(cart);
        if (result.Success)
        {
            TempData["success"] = "Email will be processed and sent shortly";
        }
        else
        {
            TempData["error"] = "Something went wrong";
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
    {
        cartDto.CartHeader.CouponCode = string.Empty;
        cartDto.CartDetails = new List<CartDetailsDto>();
        var result = await cartService.ApplyCouponAsync(cartDto);
        if (result.Success)
        {
            TempData["success"] = result.Data;
        }
        else
        {
            TempData["error"] = result.Message;
        }
        return RedirectToAction(nameof(Index));
    }


}
