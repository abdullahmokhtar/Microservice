using Microservices.Service.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Microservices.Service.RewardAPI.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Reward> Rewards { get; set; }
}
