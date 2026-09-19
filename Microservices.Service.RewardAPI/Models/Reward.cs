using System.ComponentModel.DataAnnotations;

namespace Microservices.Service.RewardAPI.Models;

public class Reward
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string UserId { get; set; }
    public DateTime RewardDate { get; set; }
    public int RewardActivity { get; set; }
    public int OrderId { get; set; }
}
