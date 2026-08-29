using System.ComponentModel.DataAnnotations;

namespace Microservices.Service.EmailAPI.Models;

public class EmailLogger
{
    public int ID { get; set; }
    [MaxLength(50)]
    public string Email { get; set; }
    [MaxLength(2000)]
    public string Message { get; set; }
    public DateTime? SentAt { get; set; }
}
