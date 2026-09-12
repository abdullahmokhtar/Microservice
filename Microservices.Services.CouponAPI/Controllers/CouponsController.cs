using System.Net;
using AutoMapper;
using Microservices.Services.CouponAPI.Data;
using Microservices.Services.CouponAPI.Models;
using Microservices.Services.CouponAPI.Models.Dto;
using Microservices.Services.CouponAPI.Utlity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Microservices.Services.CouponAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CouponsController(AppDbContext context, IMapper mapper, IOptions<StripeApiKey> stripeApiKey) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var result = await context.Coupons.AsNoTracking().ToListAsync(cancellationToken);
        return Ok(ResultDto<IEnumerable<CouponDto>>.SuccessResult(mapper.Map<List<CouponDto>>(result)));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken = default)
    {
        var coupon = await context.Coupons.FindAsync(id, cancellationToken);
        if (coupon is null) return NotFound(ResultDto<CouponDto>.FailureResult($"No coupon found with id = {id}"));
        return Ok(ResultDto<CouponDto>.SuccessResult(mapper.Map<CouponDto>(coupon)));
    }

    [HttpGet("GetByCode/{code}")]
    public async Task<IActionResult> Get(string code, CancellationToken cancellationToken = default)
    {
        var coupon = await context.Coupons.AsNoTracking().FirstOrDefaultAsync(c => c.CouponCode.Trim().ToLower() == code.Trim().ToLower(), cancellationToken);
        if (coupon is null) return NotFound(ResultDto<CouponDto>.FailureResult($"No coupon found with code = {code}"));
        return Ok(ResultDto<CouponDto>.SuccessResult(mapper.Map<CouponDto>(coupon)));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post(CouponDto couponDto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ResultDto<CouponDto>.FailureResult("Invalid Data"));

        var coupon = mapper.Map<Coupon>(couponDto);

        context.Coupons.Add(coupon);
        await context.SaveChangesAsync(cancellationToken);
        ResultDto<CouponDto> ResultDto = ResultDto<CouponDto>.SuccessResult(mapper.Map<CouponDto>(coupon));

        var options = new Stripe.CouponCreateOptions
        {
            Duration = "once",
            Id = coupon.CouponCode,
            AmountOff = (long)(coupon.DiscountAmount * 100),
            Name = coupon.CouponCode,
            Currency = "egp",
        };
        var client = new Stripe.StripeClient(stripeApiKey.Value.ApiKey);
        var service = client.V1.Coupons;
        await service.CreateAsync(options, cancellationToken: cancellationToken);
        return StatusCode((int)HttpStatusCode.Created, ResultDto);
    }

    [HttpPut]
    public async Task<IActionResult> Put(CouponDto couponDto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ResultDto<CouponDto>.FailureResult("Invalid Data"));

        var coupon = mapper.Map<Coupon>(couponDto);

        context.Coupons.Update(coupon);
        await context.SaveChangesAsync(cancellationToken);
        ResultDto<CouponDto> ResultDto = ResultDto<CouponDto>.SuccessResult(mapper.Map<CouponDto>(coupon));

        var options = new Stripe.CouponUpdateOptions
        {
            CurrencyOptions = new Dictionary<string, Stripe.CouponCurrencyOptionsOptions>
            {
                { "egp", new Stripe.CouponCurrencyOptionsOptions { AmountOff = (long)(coupon.DiscountAmount * 100) } }
            }
        };
        var client = new Stripe.StripeClient(stripeApiKey.Value.ApiKey);
        var service = client.V1.Coupons;
        await service.UpdateAsync(couponDto.CouponCode, options, cancellationToken: cancellationToken);
        return Ok(ResultDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var coupon = await context.Coupons.FirstOrDefaultAsync(c => c.CouponId == id, cancellationToken);
        if (coupon is null)
            return NotFound(ResultDto<CouponDto>.FailureResult($"Could not find coupon with id {id}"));
        context.Coupons.Remove(coupon);
        var rowsaffcted = await context.SaveChangesAsync(cancellationToken);
        if (rowsaffcted == 0)
            return StatusCode((int)HttpStatusCode.InternalServerError, ResultDto<CouponDto>.FailureResult($"Could not delete coupon with id {id}"));
        var client = new Stripe.StripeClient(stripeApiKey.Value.ApiKey);
        var service = client.V1.Coupons;
        await service.DeleteAsync(coupon.CouponCode, cancellationToken: cancellationToken);
        return Ok(ResultDto<bool>.SuccessResult(true, $"Coupon with id {id} deleted successfully"));
    }
}
