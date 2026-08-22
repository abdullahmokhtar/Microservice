using System.ComponentModel.DataAnnotations;

namespace Microservices.Services.ShoppingCartAPI.Models.Dto;

public class ProductDto()
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    [Range(1, 1000)]
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string CategoryName { get; set; }
    public string ImageUrl { get; set; }
}
