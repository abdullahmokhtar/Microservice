using System.ComponentModel.DataAnnotations;

namespace Microservices.Services.ProductAPI.Models;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    [Range(1, 1000)]
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string CategoryName { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageLocalPath { get; set; }
}
