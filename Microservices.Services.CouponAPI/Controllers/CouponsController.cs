using Microservices.Services.CouponAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Microservices.Services.CouponAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CouponsController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        => Ok(await context.Coupons.ToListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken = default)
    {
        var coupon = await context.Coupons.FindAsync( id , cancellationToken);
        if (coupon is null) return NotFound();
        return Ok(coupon);
    }
}
