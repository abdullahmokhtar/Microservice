using System.ComponentModel.DataAnnotations;

namespace Microservices.Service.OrderAPI.Models.Dto;

public class ProductDto()
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    [Range(1, 1000)]
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string CategoryName { get; set; }
    public string ImageUrl { get; set; }

    [Range(1, 100)]
    public int Count { get; set; } = 1;
}
