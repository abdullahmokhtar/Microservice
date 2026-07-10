using System.Net;
using AutoMapper;
using Microservices.Services.CouponAPI.Data;
using Microservices.Services.CouponAPI.Models;
using Microservices.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Microservices.Services.CouponAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CouponsController(AppDbContext context, IMapper mapper) : ControllerBase
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
    public async Task<IActionResult> Post(CouponDto couponDto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ResultDto<CouponDto>.FailureResult("Invalid Data"));

        var coupon = mapper.Map<Coupon>(couponDto);

        context.Coupons.Add(coupon);
        await context.SaveChangesAsync(cancellationToken);
        ResultDto<CouponDto> ResultDto = ResultDto<CouponDto>.SuccessResult(mapper.Map<CouponDto>(coupon));
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
        return Ok(ResultDto<bool>.SuccessResult(true, $"Coupon with id {id} deleted successfully"));
    }
}
