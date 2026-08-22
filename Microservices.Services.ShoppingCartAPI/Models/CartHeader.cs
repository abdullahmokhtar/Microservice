using System.ComponentModel.DataAnnotations.Schema;

namespace Microservices.Services.ShoppingCartAPI.Models;

public class CartHeader
{
    public int CartHeaderId { get; set; }
    public string? UserId { get; set; }
    public string? CouponCode { get; set; }
    public ICollection<CartDetails> CartDetails { get; set; }

    [NotMapped]
    public double Discount { get; set; }
    [NotMapped]
    public double CartTotal { get; set; }
}
