using System.ComponentModel.DataAnnotations;

namespace Microservices.Service.OrderAPI.Models.Dto;

public class CartHeaderDto
{
    public int CartHeaderId { get; set; }
    public string? UserId { get; set; }
    public string? CouponCode { get; set; }

    public decimal Discount { get; set; }
    public decimal CartTotal { get; set; }

    public string Name { get; set; }
    public string Phone { get; set; }
    [EmailAddress]
    public string Email { get; set; }
}
