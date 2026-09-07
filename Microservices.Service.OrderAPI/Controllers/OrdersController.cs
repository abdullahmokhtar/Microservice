using AutoMapper;
using Microservices.Service.OrderAPI.Data;
using Microservices.Service.OrderAPI.Models;
using Microservices.Service.OrderAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microservices.Service.OrderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController(IMapper mapper, AppDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder(CartDto cart, CancellationToken cancellationToken = default)
    {
        var orderHeaderDto = mapper.Map<OrderHeaderDto>(cart.CartHeader);
        orderHeaderDto.OrderDetails = mapper.Map<IEnumerable<OrderDetailDto>>(cart.CartDetails);

        var orderHeader = mapper.Map<OrderHeader>(orderHeaderDto);
        context.OrderHeaders.Add(orderHeader);
        await context.SaveChangesAsync(cancellationToken);
        orderHeaderDto.OrderHeaderId = orderHeader.OrderHeaderId;
        return Ok(ResultDto<OrderHeaderDto>.SuccessResult(orderHeaderDto));
    }
}
