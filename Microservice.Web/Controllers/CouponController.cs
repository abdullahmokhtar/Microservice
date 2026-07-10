using Microservice.Web.Models;
using Microservice.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Web.Controllers
{
    public class CouponController(ICouponService couponService) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            var response = await couponService.GetAllCouponsAsync();
            if (!response.Success)
            {
                TempData["error"] = response.Message;
            }
            return View(response.Data ?? new List<CouponDto>());
        }

        public IActionResult CreateCoupon()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon(CouponDto model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var response = await couponService.CreateCouponAsync(model);
            if (response.Success)
            {
                TempData["success"] = response.Message;
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = response.Message;
            return View(model);
        }

        public async Task<IActionResult> DeleteCoupon(int couponId)
        {
            var response = await couponService.GetCouponByIdAsync(couponId);
            if (!response.Success)
            {
                TempData["error"] = response.Message;
                return NotFound();
            }
            return View(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCoupon(CouponDto model)
        {
            var response = await couponService.DeleteCouponAsync(model.CouponId);
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
