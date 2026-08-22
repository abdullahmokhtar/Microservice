using System.ComponentModel.DataAnnotations.Schema;
using Microservices.Services.ShoppingCartAPI.Models.Dto;

namespace Microservices.Services.ShoppingCartAPI.Models;

public class CartDetails
{
    public int CartDetailsId { get; set; }
    public int CartHeaderId { get; set; }
    public CartHeader CartHeader { get; set; }
    public int ProductId { get; set; }
    [NotMapped]
    public ProductDto Product { get; set; }
    public int Count { get; set; }
}
