using System.ComponentModel.DataAnnotations;
using Microservices.Service.OrderAPI.Utlity;

namespace Microservices.Service.OrderAPI.Models;

public class OrderHeader
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
    public IEnumerable<OrderDetail> OrderDetails { get; set; }
}
