using System.ComponentModel.DataAnnotations;

namespace Microservices.Services.CouponAPI.Models;

public class Coupon
{
    [Key]
    public int CouponId { get; set; }
    [MaxLength(50)]
    public string CouponCode { get; set; } = null!;
    public double DiscountAmount { get; set; }
    public int MinAmount { get; set; }
}
