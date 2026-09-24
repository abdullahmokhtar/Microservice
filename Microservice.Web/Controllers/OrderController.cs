using Microservice.Web.Models;
using Microservice.Web.Services.IServices;
using Microservice.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Web.Controllers;

[Authorize]
public class OrderController(IOrderService orderService) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View();
    }

    public async Task<IActionResult> Details(int orderId)
    {
        var order = await orderService.Get(orderId);
        if (order?.Data?.UserId != User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value && !User.IsInRole(SD.RoleAdmin))
        {
            return Forbid();
        }

        return View(order.Data);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(OrderStatus? status, CancellationToken cancellationToken = default)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        var orders = await orderService.GetAll(userId, status, cancellationToken);
        return Ok(orders);
    }

    [HttpPost("ReadyForPickup")]
    public async Task<IActionResult> ReadyForPickup(int orderid, CancellationToken cancellationToken = default)
    {
        var result = await orderService.UpdateStatus(orderid, OrderStatus.ReadyForPickup, cancellationToken);
        if (result.Success)
        {
            TempData["success"] = result.Message;
        }
        return RedirectToAction(nameof(Details), new { orderId = orderid });
    }

    [HttpPost("Complete")]
    public async Task<IActionResult> Complete(int orderid, CancellationToken cancellationToken = default)
    {
        var result = await orderService.UpdateStatus(orderid, OrderStatus.Completed, cancellationToken);
        if (result.Success)
        {
            TempData["success"] = result.Message;
            return RedirectToAction(nameof(Details), new { orderId = orderid });
        }
        return View(result);
    }

    [HttpPost("Cancel")]
    public async Task<IActionResult> Cancel(int orderid, CancellationToken cancellationToken = default)
    {
        var result = await orderService.UpdateStatus(orderid, OrderStatus.Cancelled, cancellationToken);
        if (result.Success)
        {
            TempData["success"] = result.Message;
            return RedirectToAction(nameof(Details), new { orderId = orderid });
        }
        return View(result);
    }
}
