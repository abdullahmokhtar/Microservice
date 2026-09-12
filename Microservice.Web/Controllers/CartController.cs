using System.Security.Claims;
using Microservice.Web.Models;
using Microservice.Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Web.Controllers;

[Authorize]
public class CartController(ICartService cartService, IOrderService orderService) : Controller
{
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

    public async Task<IActionResult> Checkout()
    {
        return View(await GetCart());
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(CartDto cartDto, CancellationToken cancellationToken = default)
    {
        var cart = await GetCart();
        cart.CartHeader.Phone = cartDto.CartHeader.Phone;
        cart.CartHeader.Email = cartDto.CartHeader.Email;
        cart.CartHeader.Name = cartDto.CartHeader.Name;
        var result = await orderService.CreateOrderAsync(cart, cancellationToken);
        if (result is not null && result.Success)
        {
            var domain = Request.Scheme + "://" + Request.Host.Value;
            var stripeRequest = new StripeRequestDto
            {
                ApprovedURL = domain + "/Cart/Confirmation/Cart?orderId=" + result.Data.OrderHeaderId,
                CancelURL = domain + Url.Action(nameof(Checkout), "Cart"),
                OrderHeader = result.Data
            };
            var stripeResult = await orderService.CreateStripeSessionAsync(stripeRequest, cancellationToken);
            return Redirect(stripeResult.Data.SessionURL);
        }

        return View();
    }

    public async Task<IActionResult> Confirmation(int orderId, CancellationToken cancellationToken = default)
    {
        var result = await orderService.ValidateStipeSessionAsync(orderId, cancellationToken);
        if (result.Success && result.Data.Status == OrderStatus.Approved)
        {
            return View(orderId);
        }
        return View(orderId);
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
        cart?.CartHeader.Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
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
