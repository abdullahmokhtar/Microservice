using System.ComponentModel.DataAnnotations;

namespace Microservice.Web.Models;

public class OrderHeaderDto
{
    public int OrderHeaderId { get; set; }
    public string? UserId { get; set; }
    public string? CouponCode { get; set; }
    public decimal Discount { get; set; }
    public decimal OrderTotal { get; set; }

    public string Name { get; set; }
    public string Phone { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? PaymentIntntId { get; set; }
    public string? StripeSessionId { get; set; }
    public IEnumerable<OrderDetailDto> OrderDetails { get; set; } = [];
}
