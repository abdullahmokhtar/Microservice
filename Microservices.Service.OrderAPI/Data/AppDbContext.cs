using Microservices.Service.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Microservices.Service.OrderAPI.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<OrderHeader> OrderHeaders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
}
