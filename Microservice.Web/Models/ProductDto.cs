using System.ComponentModel.DataAnnotations;
using Microservice.Web.Attributes;

namespace Microservice.Web.Models;

public class ProductDto()
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    [Range(1, 1000)]
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string CategoryName { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageLocalPath { get; set; }

    [Range(1, 100)]
    public int Count { get; set; } = 1;
    [ValidateImageFile]
    public IFormFile? Image { get; set; }
}
