using System.ComponentModel.DataAnnotations.Schema;
using Microservices.Service.OrderAPI.Models.Dto;

namespace Microservices.Service.OrderAPI.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderHeaderId { get; set; }
        public OrderHeader OrderHeader { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public ProductDto? Product { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }
}
